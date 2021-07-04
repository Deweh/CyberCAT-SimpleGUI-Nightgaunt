using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CyberCAT.SimpleGUI.MVVM.View
{
    public static class CanvasExtensions
    {
        public static readonly DependencyProperty CenterRelativeProperty =
            DependencyProperty.RegisterAttached("CenterRelative", typeof(bool), typeof(CanvasExtensions), new PropertyMetadata(false));

        public static readonly DependencyProperty RelativeLeftProperty =
            DependencyProperty.RegisterAttached("RelativeLeft", typeof(double), typeof(CanvasExtensions), new PropertyMetadata(0.0, new PropertyChangedCallback(OnRelativeLeftChanged)));

        public static readonly DependencyProperty RelativeRightProperty =
            DependencyProperty.RegisterAttached("RelativeRight", typeof(double), typeof(CanvasExtensions), new PropertyMetadata(0.0, new PropertyChangedCallback(OnRelativeRightChanged)));

        public static readonly DependencyProperty RelativeTopProperty =
            DependencyProperty.RegisterAttached("RelativeTop", typeof(double), typeof(CanvasExtensions), new PropertyMetadata(0.0, new PropertyChangedCallback(OnRelativeTopChanged)));

        public static readonly DependencyProperty RelativeBottomProperty =
            DependencyProperty.RegisterAttached("RelativeBottom", typeof(double), typeof(CanvasExtensions), new PropertyMetadata(0.0, new PropertyChangedCallback(OnRelativeBottomChanged)));

        public static bool GetCenterRelative(DependencyObject obj)
        {
            return (bool)obj.GetValue(CenterRelativeProperty);
        }

        public static void SetCenterRelative(DependencyObject obj, bool value)
        {
            obj.SetValue(CenterRelativeProperty, value);
        }

        public static double GetRelativeLeft(DependencyObject obj)
        {
            return (double)obj.GetValue(RelativeLeftProperty);
        }

        public static void SetRelativeLeft(DependencyObject obj, double value)
        {
            obj.SetValue(RelativeLeftProperty, value);
        }

        public static double GetRelativeTop(DependencyObject obj)
        {
            return (double)obj.GetValue(RelativeTopProperty);
        }

        public static void SetRelativeTop(DependencyObject obj, double value)
        {
            obj.SetValue(RelativeTopProperty, value);
        }

        public static double GetRelativeRight(DependencyObject obj)
        {
            return (double)obj.GetValue(RelativeRightProperty);
        }

        public static void SetRelativeRight(DependencyObject obj, double value)
        {
            obj.SetValue(RelativeRightProperty, value);
        }

        public static double GetRelativeBottom(DependencyObject obj)
        {
            return (double)obj.GetValue(RelativeBottomProperty);
        }

        public static void SetRelativeBottom(DependencyObject obj, double value)
        {
            obj.SetValue(RelativeBottomProperty, value);
        }

        private static void OnRelativeLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement element)) return;
            if (!(VisualTreeHelper.GetParent(element) is Canvas canvas)) return;

            canvas.SizeChanged += (s, arg) =>
            {
                double relativeLeftPosition = GetRelativeLeft(element);
                double leftPosition = relativeLeftPosition * canvas.ActualWidth;

                if (GetCenterRelative(element))
                {
                    leftPosition -= element.ActualWidth / 2;
                }

                Canvas.SetLeft(element, leftPosition);
            };
        }

        private static void OnRelativeRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement element)) return;
            if (!(VisualTreeHelper.GetParent(element) is Canvas canvas)) return;

            canvas.SizeChanged += (s, arg) =>
            {
                double relativeRightPosition = GetRelativeRight(element);
                double rightPosition = relativeRightPosition * canvas.ActualWidth;

                if (GetCenterRelative(element))
                {
                    rightPosition -= element.ActualWidth / 2;
                }

                Canvas.SetRight(element, rightPosition);
            };
        }

        private static void OnRelativeTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement element)) return;
            if (!(VisualTreeHelper.GetParent(element) is Canvas canvas)) return;

            canvas.SizeChanged += (s, arg) =>
            {
                double relativeTopPosition = GetRelativeTop(element);
                double topPosition = relativeTopPosition * canvas.ActualHeight;

                if (GetCenterRelative(element))
                {
                    topPosition -= element.ActualHeight / 2;
                }

                Canvas.SetTop(element, topPosition);
            };
        }

        private static void OnRelativeBottomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement element)) return;
            if (!(VisualTreeHelper.GetParent(element) is Canvas canvas)) return;

            canvas.SizeChanged += (s, arg) =>
            {
                double relativeBottomPosition = GetRelativeBottom(element);
                double bottomPosition = relativeBottomPosition * canvas.ActualHeight;

                if (GetCenterRelative(element))
                {
                    bottomPosition -= element.ActualHeight / 2;
                }

                Canvas.SetBottom(element, bottomPosition);
            };
        }
    }
}
