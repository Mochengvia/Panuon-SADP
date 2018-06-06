using Caliburn.Micro;
using Panuon.UI;
using Panuon_SADP_net40.Logic;
using Panuon_SADP_net40.Models;
using Panuon_SADP_net40.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Panuon_SADP_net40.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<SortingCanvasViewModel>.Collection.OneActive, IShell
    {
        #region 
        private readonly IWindowManager _windowManager;

        private SortCategory _sortCategory = SortCategory.InsertSort;

        private enum ModeCategory
        {
            Running = 0,
            SingleStep = 1,
            Paused = 2,
            Stoped = 3,
        }
        private ModeCategory _currentMode = ModeCategory.Stoped;

        private Thread _thread;

        private List<int> _currentData;
        #endregion

        public ShellViewModel()
        {
            _windowManager = IoC.Get<IWindowManager>();

            ActivateItem(new SortingCanvasViewModel());
        }

        #region Bindings

        /// <summary>
        /// 数据 输入框水印
        /// </summary>
        public string WaterMark
        {
            get { return _waterMark; }
            set { _waterMark = value; NotifyOfPropertyChange(() => WaterMark); }
        }
        private string _waterMark = "仅支持整数，相邻数字使用逗号（中英文都可）隔开。\n\n非法的单个数据将被丢弃。";

        /// <summary>
        /// 数据输入框内容
        /// </summary>
        public string InputDataString
        {
            get { return _inputDataString; }
            set { _inputDataString = value; NotifyOfPropertyChange(() => InputDataString); }
        }
        private string _inputDataString = "";

        /// <summary>
        /// 随机数输入框内容
        /// </summary>
        public string InputQuantityString
        {
            get { return _inputQuantityString; }
            set { _inputQuantityString = value; NotifyOfPropertyChange(() => InputQuantityString); }
        }
        private string _inputQuantityString = "15";

        /// <summary>
        /// 随机数输入框内容
        /// </summary>
        public string ConsoleContent
        {
            get { return _consoleContent; }
            set { _consoleContent = value; NotifyOfPropertyChange(() => ConsoleContent); }
        }
        private string _consoleContent = "无输出";

        /// <summary>
        /// 使用随机数CheckBox勾选状态
        /// </summary>
        public bool IsUsingRandomChecked
        {
            get { return _isUsingRandomChecked; }
            set { _isUsingRandomChecked = value; NotifyOfPropertyChange(() => IsUsingRandomChecked); }
        }
        private bool _isUsingRandomChecked = true;

        /// <summary>
        /// 执行间隔
        /// </summary>
        public int ExcuteDuration
        {
            get { return _ExcuteDuration; }
            set { _ExcuteDuration = value; NotifyOfPropertyChange(() => ExcuteDuration); }
        }
        private int _ExcuteDuration = 50;

        /// <summary>
        /// 排序类型RadioButtons的可用性
        /// </summary>
        public bool IsSortCategoriesEnabled
        {
            get { return _isSortCategoriesEnabled; }
            set { _isSortCategoriesEnabled = value; NotifyOfPropertyChange(() => IsSortCategoriesEnabled); }
        }
        public bool _isSortCategoriesEnabled = true;

        /// <summary>
        /// 生成按钮可用
        /// </summary>
        public bool IsGenerateButtonEnabled
        {
            get { return _isGenerateButtonEnabled; }
            set { _isGenerateButtonEnabled = value; NotifyOfPropertyChange(() => IsGenerateButtonEnabled); }
        }
        public bool _isGenerateButtonEnabled = true;

        /// <summary>
        /// 执行按钮可用
        /// </summary>
        public bool IsRunButtonEnabled
        {
            get { return _isRunButtonEnabled; }
            set { _isRunButtonEnabled = value; NotifyOfPropertyChange(() => IsRunButtonEnabled); }
        }
        public bool _isRunButtonEnabled = true;

        /// <summary>
        /// 单步按钮可用
        /// </summary>
        public bool IsSingleStepButtonEnabled
        {
            get { return _isSingleStepButtonEnabled; }
            set { _isSingleStepButtonEnabled = value; NotifyOfPropertyChange(() => IsSingleStepButtonEnabled); }
        }
        public bool _isSingleStepButtonEnabled = true;

        /// <summary>
        /// 暂停按钮可用
        /// </summary>
        public bool IsPauseButtonEnabled
        {
            get { return _isPauseButtonEnabled; }
            set { _isPauseButtonEnabled = value; NotifyOfPropertyChange(() => IsPauseButtonEnabled); }
        }
        public bool _isPauseButtonEnabled = false;

        /// <summary>
        /// 停止按钮可用
        /// </summary>
        public bool IsStopButtonEnabled
        {
            get { return _isStopButtonEnabled; }
            set { _isStopButtonEnabled = value; NotifyOfPropertyChange(() => IsStopButtonEnabled); }
        }
        public bool _isStopButtonEnabled = false;
        #endregion

        #region Events

        public void Exit()
        {
            Environment.Exit(0);
        }

        public void SizeChanged()
        {
            ActiveItem.ClearCanvas();
            if (_currentData != null)
                ActiveItem.InitCanvas(_currentData);
        }

        //用户选择了新算法。
        public void SortCategoryCheckedChanged(int id)
        {
            _sortCategory = (SortCategory)id;
            if(_currentData != null)
                ConsoleContent = $"生成成功，共获取到{_currentData.Count}个数据。使用{_sortCategory}大约需要{SortStepGenerator.Sort(_sortCategory, _currentData)?.Where(x => x.Category != StepCategory.ChangeIndex).Count() - 1}步。";

        }

        // 生成操作
        public void Generate()
        {
            if (InputDataString == "" || IsUsingRandomChecked)
            {
                //尝试转换为Int
                var quantity = 0;
                if (Int32.TryParse(InputQuantityString, out quantity))
                    InputDataString = NumbersToString(GetRandomNumbers(quantity));
                else
                {
                    InputQuantityString = "15";
                    InputDataString = NumbersToString(GetRandomNumbers(15));
                }
                IsUsingRandomChecked = true;
            }
            else
            {
                InputDataString = InputDataString.Replace(',', '，');
                if (InputDataString.Split('，').Length - 1 <= 1)
                {
                    ConsoleContent = $"可用数据的数量低于最小值2。";
                    return;
                }
            }
            _currentData = StringToNumbers(InputDataString).ToList();
            if (_currentData.Min() == _currentData.Max())
            {
                ConsoleContent = $"无法对多个相同数据执行排序。";
                return;

            }

            ActiveItem.InitCanvas(_currentData);
            SetButtonsEnable();
            IsSortCategoriesEnabled = true;
            ConsoleContent = $"生成成功，共获取到{_currentData.Count}个数据。使用{_sortCategory}大约需要{SortStepGenerator.Sort(_sortCategory, _currentData)?.Where(x => x.Category != StepCategory.ChangeIndex).Count() - 1}步。";
        }

        //清除画布
        public void Clear()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive && _thread.ThreadState == ThreadState.Suspended)
                    _thread.Resume();
                _thread.Abort();
            }
            _currentData = null;
            _currentMode = ModeCategory.Stoped;

            InputDataString = "";
            ActiveItem.ClearCanvas();
            SetButtonsEnable();
            IsSortCategoriesEnabled = true;
            (GetView() as PUWindow).ResizeMode = ResizeMode.CanResize;
        }

        //数据录入区KeyDown事件
        public void InputDataKeyDown(ActionExecutionContext context)
        {
            IsUsingRandomChecked = false;
            var e = (KeyEventArgs)context.EventArgs;
            if (e.Key == Key.Enter)
                Generate();
        }

        //修改随机数数量+-5
        public void ChangeRandomQuantity(int delta)
        {
            var quantity = 0;
            if (Int32.TryParse(InputQuantityString, out quantity))
                InputQuantityString = (quantity + delta).ToString();
            else
            {
                InputQuantityString = "15";
                InputQuantityString = (quantity + delta).ToString();
            }

        }

        public void Run()
        {
            //在排序任务中 - 继续执行
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Resume();
                _currentMode = ModeCategory.Running;
                ConsoleContent = "开始继续执行。";
                return;
            }
            //没有数据
            if (_currentData == null)
                Generate();
            //开始执行
            _thread = new Thread(() =>
            {
                ExecuteSteps();
            });
            
            _thread.Start();

            SetButtonsEnable(false, false, true, true, true);
            IsSortCategoriesEnabled = false;
            (GetView() as PUWindow).ResizeMode = ResizeMode.CanMinimize;
        } 

        public void SingleStep()
        {
            if (_thread != null && _thread.IsAlive)
            {
                if (_currentMode == ModeCategory.SingleStep || _currentMode == ModeCategory.Paused)
                {
                    _thread.Resume();
                }
                else
                {
                    _currentMode = ModeCategory.SingleStep;
                    SetButtonsEnable(false, true, true, false, true);
                    IsSortCategoriesEnabled = false;
                    ConsoleContent = "已进入单步模式。";
                }
                return;
            }
            //没有数据
            if (_currentData == null)
                Generate();

            _currentMode = ModeCategory.SingleStep;
            var stepList = SortStepGenerator.Sort(_sortCategory, _currentData);
            _thread = new Thread(() =>
            {
                ExecuteSteps();
            });
            _thread.Start();
            SetButtonsEnable(false, true, true, false, true);
            IsSortCategoriesEnabled = false;
            (GetView() as PUWindow).ResizeMode = ResizeMode.CanMinimize;
        }

        public void Pause()
        {
            if (_thread == null || !_thread.IsAlive)
            {
                ConsoleContent = "没有可以暂停的排序任务，请先从数据区生成画布。";
                return;
            }
            _currentMode = ModeCategory.Paused;
            IsRunButtonEnabled = true;
            IsSingleStepButtonEnabled = true;
            IsPauseButtonEnabled = false;
            ConsoleContent = "排序已暂停。";
        }

        public void Stop()
        {
            if (_thread != null && _thread.IsAlive)
            {
                try
                { _thread.Abort(); }
                catch { }
                _thread = null;
            }
            _currentMode = ModeCategory.Stoped;

            SetButtonsEnable();
            ActiveItem.ResetCanvas();
            IsSortCategoriesEnabled = true;

            ConsoleContent = "画布已重置。";
            (GetView() as PUWindow).ResizeMode = ResizeMode.CanResize;
        }

        public void GitHub()
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/Ruris/Panuon-SADP");
        }
        #endregion

        #region Function
        /// <summary>
        /// 生成随机数。
        /// </summary>
        private IList<int> GetRandomNumbers(int quantity)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            var list = new List<int>();
            while (quantity > 0)
            {
                quantity--;
                list.Add(rand.Next(1, 100));
            }
            return list;
        }

        /// <summary>
        /// 数组转化为逗号分隔字符串。
        /// </summary>
        private string NumbersToString(IEnumerable<int> datas)
        {
            return String.Join("，", datas);
        }

        /// <summary>
        /// 逗号分隔字符串转化为数组。
        /// </summary>
        private IEnumerable<int> StringToNumbers(string datas)
        {
            datas = datas.Replace(',', '，');
            var dataarray = datas.Split('，');
            foreach (var data in dataarray)
            {
                if (data != "")
                {
                    var number = 0;
                    if (Int32.TryParse(data, out number))
                        yield return number;
                }
            }
            yield break;
        }

        /// <summary>
        /// 执行排序步骤集合。
        /// </summary>
        private void ExecuteSteps()
        {
            var counter = 0;
            foreach (var step in SortStepGenerator.Sort(_sortCategory, _currentData))
            {
                var canvas = ActiveItem;

                if ((_currentMode == ModeCategory.SingleStep || _currentMode == ModeCategory.Paused) && counter != 1 && step.Category != StepCategory.ChangeIndex)
                    try { _thread.Suspend(); } catch { }

                ConsoleContent = step.Description;
                switch (step.Category)
                {
                    //移动指示器
                    case StepCategory.MoveIndivator:
                        canvas.MoveIndicator(step.From - 1, step.To);
                        canvas.SetHighLight(step.SelectedIndex);
                        break;
                    //交换条并交换索引
                    case StepCategory.SwapBar:
                        canvas.MoveBar(step.From, step.To);
                        canvas.MoveBar(step.To, step.From);
                        canvas.SwapIndex(step.From, step.To);
                        canvas.SetHighLight(step.SelectedIndex);
                        break;
                    //移动条
                    case StepCategory.MoveBar:
                        canvas.MoveBar(step.From, step.To, step.Raise);
                        canvas.SetHighLight(step.SelectedIndex);
                        break;
                    //修改索引映射
                    case StepCategory.ChangeIndex:
                        canvas.ChangeIndex(step.ChangedIndexList);
                        if (step.SkipWaiting)
                            continue;
                        break;
                    //创建桶:
                    case StepCategory.CreateBucket:
                        canvas.CreateBucket(step.BucketQuantity, step.BucketVolumes);
                        break;
                    //排序完成
                    case StepCategory.Completed:
                        SetButtonsEnable(true, false, false, false, true);
                        canvas.ResetIndicatorAndBucket();
                        canvas.SetHighLight(-2);
                        break;
                }
                counter++;
                if (_currentMode != ModeCategory.SingleStep && _currentMode != ModeCategory.Paused)
                    Thread.Sleep(ExcuteDuration);
            }

        }

      
        private void SetButtonsEnable(bool generate = true, bool run = true, bool singleStep = true, bool pause = false, bool stop = false)
        {
            IsGenerateButtonEnabled = generate;
            IsRunButtonEnabled = run;
            IsSingleStepButtonEnabled = singleStep;
            IsPauseButtonEnabled = pause;
            IsStopButtonEnabled = stop;
        }
        #endregion
    }
}
