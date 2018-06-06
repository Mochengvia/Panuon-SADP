using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Panuon_SADP_net40
{
    public class SortBar : Button
    {
        static SortBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SortBar), new FrameworkPropertyMetadata(typeof(SortBar)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Margin = new Thickness(Left, 0, 0, 0);
        }

        /// <summary>
        /// 移除自己。
        /// </summary>
        public event PropertyChangedEventHandler DeleteThis;
        internal void OnDeleteThis()
        {
            DeleteThis(this, null);
        }

        /// <summary>
        /// 内部高度，默认值为0。
        /// </summary>
        public double InnerHeight
        {
            get { return (double)GetValue(InnerHeightProperty); }
            set
            {
                if (value == -1)
                {
                    DoubleAnimation da = new DoubleAnimation() { To = 0, Duration = TimeSpan.FromSeconds(0.4), EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }, };
                    da.Completed += delegate
                    {
                        OnDeleteThis();
                    };
                    BeginAnimation(InnerHeightProperty, da);
                }
                else
                {
                    DoubleAnimation da = new DoubleAnimation() { To = value, Duration = TimeSpan.FromSeconds(0.4), EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }, };
                    BeginAnimation(InnerHeightProperty, da);
                }
               
            }
        }
        public static readonly DependencyProperty InnerHeightProperty = DependencyProperty.Register("InnerHeight", typeof(double), typeof(SortBar), new PropertyMetadata((double)0));

        /// <summary>
        /// Left属性
        /// </summary>
        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set
            {
                SetValue(LeftProperty, value);
                DoAnimationMargin(value);
            }
        }
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(SortBar), new PropertyMetadata((double)0));

        /// <summary>
        /// 是否升起
        /// </summary>
        public bool IsRaised
        {
            get { return (bool)GetValue(IsRaisedProperty); }
            set
            {
                if (value)
                    DoAnimationMargin(Left, 30);
                else
                    DoAnimationMargin(Left, 0);
                SetValue(IsRaisedProperty, value);
            }
        }
        public static readonly DependencyProperty IsRaisedProperty = DependencyProperty.Register("IsRaised", typeof(bool), typeof(SortBar), new PropertyMetadata(false));

        /// <summary>
        /// 是否高亮
        /// </summary>
        public bool IsHighLight
        {
            get { return (bool)GetValue(IsHighLightProperty); }
            set
            {
                if (!value)
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC2FA2DB"));
                else
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCC5665"));

                SetValue(IsHighLightProperty, value);
            }
        }
        public static readonly DependencyProperty IsHighLightProperty = DependencyProperty.Register("IsHighLight", typeof(bool), typeof(SortBar), new PropertyMetadata(false));


        /// <summary>
        /// 排序前的Index
        /// </summary>
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(SortBar), new PropertyMetadata(0));


        #region Private
        private void DoAnimationMargin(double left)
        {
            ThicknessAnimation da = new ThicknessAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.2),
                To = new Thickness(left, 0, 0, Margin.Bottom),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
            };
            BeginAnimation(MarginProperty, da);
        }

        private void DoAnimationMargin(double left, double bottom)
        {
            ThicknessAnimation da = new ThicknessAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.2),
                To = new Thickness(left, 0, 0, bottom),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
            };
            BeginAnimation(MarginProperty, da);
        }
        #endregion
    }


}
