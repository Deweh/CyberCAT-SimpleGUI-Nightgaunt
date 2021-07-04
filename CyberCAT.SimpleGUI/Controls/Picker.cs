using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace CyberCAT.SimpleGUI.Controls
{
    class Picker : Control
    {
        private RepeatButton downButton;
        private RepeatButton upButton;
        public TextBlock valueDisplay;
        private Grid baseGrid;
        private bool _hideBtns = false;

        public int Value
        {
            get
            {
                return GetValue(this);
            }
            set
            {
                SetValue(this, value);
            }
        }

        public bool HideButtons
        {
            get
            {
                return _hideBtns;
            }
            set
            {
                _hideBtns = value;

                if (downButton != null && upButton != null)
                {
                    if (_hideBtns)
                    {
                        downButton.Opacity = 0;
                        upButton.Opacity = 0;
                    }
                    else
                    {
                        downButton.Opacity = 1;
                        upButton.Opacity = 1;
                    }
                }
            }
        }

        static Picker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Picker), new FrameworkPropertyMetadata(typeof(Picker)));
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(int), typeof(Picker), new PropertyMetadata(0, OnValueChanged));

        public static int GetValue(DependencyObject obj)
        {
            return (int)obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, int value)
        {
            obj.SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Picker)d).valueDisplay != null)
            {
                ((Picker)d).valueDisplay.Text = e.NewValue.ToString();
            }
        }

        public override void OnApplyTemplate()
        {
            downButton = Template.FindName("PART_DownButton", this) as RepeatButton;
            upButton = Template.FindName("PART_UpButton", this) as RepeatButton;
            valueDisplay = Template.FindName("PART_ValueDisplay", this) as TextBlock;
            baseGrid = Template.FindName("PART_BaseGrid", this) as Grid;

            if (_hideBtns)
            {
                downButton.Opacity = 0;
                upButton.Opacity = 0;
            }

            downButton.Click += downButton_Click;
            upButton.Click += upButton_Click;
            baseGrid.MouseEnter += baseGrid_MouseEnter;
            baseGrid.MouseLeave += baseGrid_MouseLeave;

            valueDisplay.Text = Value.ToString();

            base.OnApplyTemplate();
        }

        private void baseGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!HideButtons)
            {
                return;
            }

            var anim = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            downButton.BeginAnimation(UIElement.OpacityProperty, anim);
            upButton.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void baseGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!HideButtons)
            {
                return;
            }

            var anim = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            downButton.BeginAnimation(UIElement.OpacityProperty, anim);
            upButton.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            Value++;
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            Value--;
        }
    }
}
