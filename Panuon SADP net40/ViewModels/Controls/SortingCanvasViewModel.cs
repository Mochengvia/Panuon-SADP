using Caliburn.Micro;
using Panuon.UI;
using Panuon_SADP_net40.Models;
using Panuon_SADP_net40.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Panuon_SADP_net40.ViewModels.Controls
{
    public class SortingCanvasViewModel : Screen, IShell
    {
        #region Private
        private double _sortBarHeight;
        private double _sortBarWidth;

        //当用户点击CLR按钮，清除动画还未之行结束就点击生成，需要重新生成SortBar。
        private bool _clearHandle;

        private int[] _indexList;

        private SortBar _lastColorChanged;

        #endregion

        public SortingCanvasViewModel()
        {
            _clearHandle = false;

            SortBarControls = new ObservableCollection<SortBar>();
            IndicatorControls = new ObservableCollection<SortBar>();
        }

        #region Bindings
        /// <summary>
        /// 排序条s
        /// </summary>
        public ObservableCollection<SortBar> SortBarControls
        {
            get { return _sortBarControls; }
            set { _sortBarControls = value; NotifyOfPropertyChange(() => SortBarControls); }
        }
        private ObservableCollection<SortBar> _sortBarControls;

        /// <summary>
        /// 指示器s
        /// </summary>
        public ObservableCollection<SortBar> IndicatorControls
        {
            get { return _indicatorControls; }
            set { _indicatorControls = value; NotifyOfPropertyChange(() => IndicatorControls); }
        }
        private ObservableCollection<SortBar> _indicatorControls;

        /// <summary>
        /// 桶s
        /// </summary>
        public ObservableCollection<SortBar> BucketControls
        {
            get { return _bucketControls; }
            set { _bucketControls = value; NotifyOfPropertyChange(() => BucketControls); }
        }
        private ObservableCollection<SortBar> _bucketControls;
        #endregion


        #region APIs
        /// <summary>
        /// 初始化画布。
        /// </summary>
        public void InitCanvas(IList<int> data)
        {
            ResetIndexList(data.Count);
            //上左右留边距20
            _sortBarHeight = ((GetView() as SortingCanvasView).ActualHeight - 20);
            _sortBarWidth = ((GetView() as SortingCanvasView).ActualWidth - 40 - data.Count()) / data.Count();

            var minvalue = data.Min();
            //高度增量（data每增加1）,最小高度30，上边距20，去掉50
            var delta = (_sortBarHeight - 50) / (data.Max() - minvalue);

            //如果已有同样数量的控件，不再次生成（在Clear动画执行时需要重新生成）
            if (SortBarControls.Count == data.Count && !_clearHandle)
            {
                for (var i = 0; i < data.Count(); i++)
                {
                    SortBarControls[i].InnerHeight = GetInnerHeight(data[i], delta, minvalue);
                    SortBarControls[i].Content = data[i];
                    SortBarControls[i].ToolTip = data[i];
                    SortBarControls[i].Left = 20 + i * (_sortBarWidth + 1);
                }
            }
            else
            {
                SortBarControls.Clear();
                for (var i = 0; i < data.Count(); i++)
                {
                    var bar = new SortBar()
                    {
                        Height = _sortBarHeight,
                        Width = _sortBarWidth,
                        Left = 20 + i * (_sortBarWidth + 1),
                        Content = data[i],
                        InnerHeight = GetInnerHeight(data[i], delta, minvalue),
                        Index = i,
                        ToolTip = data[i],
                    };

                    bar.DeleteThis += delegate
                    {
                        SortBarControls.Remove(bar);
                    };
                    SortBarControls.Add(bar);
                }
            }

            IndicatorControls = new ObservableCollection<SortBar>()
            {
                new SortBar()
                {
                    Height = _sortBarHeight + 20,
                    Width = _sortBarWidth,
                    BorderThickness = new System.Windows.Thickness(1),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5D8CD0")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5D8CD0")),
                    Left = -1 * _sortBarWidth ,
                    Content = "A",
                    VerticalContentAlignment = System.Windows.VerticalAlignment.Top,
                },
                new SortBar()
                {
                    Height = _sortBarHeight + 20,
                    Width = _sortBarWidth,
                    BorderThickness = new System.Windows.Thickness(1),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE28153")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE28153")),
                    Left = -1 * _sortBarWidth,
                    Content = "B",
                    VerticalContentAlignment = System.Windows.VerticalAlignment.Top,
                },
            };
        }

        /// <summary>
        /// 清空元素，重置指示器和桶。
        /// </summary>
        public void ClearCanvas()
        {
            _clearHandle = true;
            if (SortBarControls != null)
                ResetSortBars();
            if (IndicatorControls != null)
                ResetIndicatorAndBucket();

            DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.4) };
            timer.Tick += delegate { _clearHandle = false; timer.Stop(); };
            timer.Start();
        }

        /// <summary>
        /// 复原排序条、指示器的位置和桶。
        /// </summary>
        public void ResetCanvas()
        {
            (GetView() as SortingCanvasView).Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Forms.MethodInvoker(delegate ()
            {
                foreach (var bar in SortBarControls)
                {
                    bar.Left = bar.Index * _sortBarWidth + 20 + bar.Index;
                    bar.IsRaised = false;
                    bar.IsHighLight = false;
                }
            }));

            ResetIndexList(SortBarControls.Count);
            ResetIndicatorAndBucket();
        }

        /// <summary>
        /// 设置排序条的位置。
        /// </summary>
        public void MoveBar(int index1, int index2, int rasie = 0)
        {
            (GetView() as SortingCanvasView).Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Forms.MethodInvoker(delegate ()
            {
                var bar = SortBarControls[_indexList[index1]] as SortBar;
                bar.Left = index2 * _sortBarWidth + 20 + index2;
                if (rasie == 1)
                    bar.IsRaised = true;
                else if (rasie == -1)
                    bar.IsRaised = false;
            }));
        }

        /// <summary>
        /// 移动指示器。
        /// </summary>
        public void MoveIndicator(int indicatorIndex, int to)
        {
            (GetView() as SortingCanvasView).Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Forms.MethodInvoker(delegate ()
            {
                IndicatorControls[indicatorIndex].Left = to * _sortBarWidth + 20 + to;
            }));
        }

        /// <summary>
        /// 设置排序条高亮。
        /// -2清空上一个 -1无操作 >=0设置指定的Bar
        /// </summary>
        public void SetHighLight(int index)
        {
            (GetView() as SortingCanvasView).Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Forms.MethodInvoker(delegate ()
            {
                switch (index)
                {
                    //清空
                    case -2:
                        if (_lastColorChanged != null)
                            _lastColorChanged.IsHighLight = false;
                        break;
                    //无操作
                    case -1:
                        break;
                    //正常索引
                    default:
                        if (_lastColorChanged != null)
                        {
                            if (_lastColorChanged.Index == _indexList[index])
                                return;
                            _lastColorChanged.IsHighLight = false;
                        }
                        _lastColorChanged = SortBarControls[_indexList[index]] as SortBar;
                        _lastColorChanged.IsHighLight = true;
                        break;
                }
            }));
        }

        /// <summary>
        /// 交换索引。
        /// </summary>
        public void SwapIndex(int index1, int index2)
        {
            var temp = _indexList[index2];
            _indexList[index2] = _indexList[index1];
            _indexList[index1] = temp;
        }

        /// <summary>
        /// 修改index1的索引映射为index2（而不交换）。
        /// </summary>
        public void ChangeIndex(int[] changedIndex)
        {
            _indexList = changedIndex;
        }

        /// <summary>
        /// 重置指示器的位置。
        /// </summary>
        public void ResetIndicatorAndBucket()
        {
            (GetView() as SortingCanvasView).Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Forms.MethodInvoker(delegate ()
            {
                IndicatorControls.Apply(x => x.Left = -1 * _sortBarWidth);
                if (BucketControls != null)
                    BucketControls.Apply(x => x.InnerHeight = 0);
            }));
        }

        public void CreateBucket(int bucketQuantity, int[] bucketVolumes)
        {
            (GetView() as SortingCanvasView).Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Forms.MethodInvoker(delegate ()
            {

                var count = 0;

                BucketControls = new ObservableCollection<SortBar>();
                for (int i = 0; i < bucketVolumes.Length; i++)
                {
                    if (bucketVolumes[i] == 0)
                        continue;
                    var bucket = new SortBar()
                    {
                        Width = (_sortBarWidth + 1) * bucketVolumes[i],
                        Height = _sortBarHeight,
                        BorderThickness = new Thickness(1),
                        InnerHeight = _sortBarHeight,
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#55222222")),
                        Content = $"   桶{i + 1}\n({i * 10}~{i * 10 + 9})",
                        VerticalContentAlignment = VerticalAlignment.Top,
                        Left = count * (_sortBarWidth + 1) + 20,
                    };
                    BucketControls.Add(bucket);
                    count += bucketVolumes[i];
                }
            }));
        }

        #endregion


        #region Function
        /// <summary>
        /// 计算条的内部高度。
        /// </summary>
        /// <param name="data">数据。</param>
        /// <param name="delta">高度增量。</param>
        /// <param name="min">最小值。</param>
        /// <returns></returns>
        private double GetInnerHeight(int data, double delta, int min)
        {
            return 30 + (data - min) * delta;
        }

        private void ResetIndexList(int count)
        {
            _indexList = new int[count];
            for (int i = 0; i < count; i++)
                _indexList[i] = i;
        }

        /// <summary>
        /// 重置排序条
        /// </summary>
            private void ResetSortBars()
        {
            (GetView() as SortingCanvasView).Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Forms.MethodInvoker(delegate ()
            {
                SortBarControls.Apply(x => x.InnerHeight = -1);
            }));
        }


        #endregion
    }
}
