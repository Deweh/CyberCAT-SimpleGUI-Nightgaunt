using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace CyberCAT.SimpleGUI.Controls
{
    class Picker : Control
    {
        private RepeatButton downButton;
        private RepeatButton upButton;
        private Grid baseGrid;
        private bool _hideBtns = false;

        static Picker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Picker), new FrameworkPropertyMetadata(typeof(Picker)));
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

        public string Formatting { get; set; } = null;

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string StringValue
        {
            get { return (string)GetValue(StringValueProperty); }
            set { SetValue(StringValueProperty, value); }
        }

        public string[] StringCollection
        {
            get { return (string[])GetValue(StringCollectionProperty); }
            set { SetValue(StringCollectionProperty, value); }
        }

        public DisplayDataType DataType
        {
            get { return (DisplayDataType)GetValue(DataTypeProperty); }
            set { SetValue(DataTypeProperty, value); }
        }

        public UpdateStringMode UpdateStringValueMode
        {
            get { return (UpdateStringMode)GetValue(UpdateStringValueModeProperty); }
            set { SetValue(UpdateStringValueModeProperty, value); }
        }

        public static readonly DependencyProperty UpdateStringValueModeProperty =
            DependencyProperty.Register("UpdateStringValueMode", typeof(UpdateStringMode), typeof(Picker), new PropertyMetadata(UpdateStringMode.Auto));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(Picker), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, ValueChanged, ValueCoerced, false, UpdateSourceTrigger.Explicit));

        public static readonly DependencyProperty StringValueProperty =
            DependencyProperty.Register("StringValue", typeof(string), typeof(Picker), new PropertyMetadata("0"));

        public static readonly DependencyProperty StringCollectionProperty =
            DependencyProperty.Register("StringCollection", typeof(string[]), typeof(Picker), new PropertyMetadata(Array.Empty<string>(), ValueDependencyChanged));

        public static readonly DependencyProperty DataTypeProperty =
            DependencyProperty.Register("DataType", typeof(DisplayDataType), typeof(Picker), new PropertyMetadata(DisplayDataType.Integer, ValueDependencyChanged));

        private static void ValueDependencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Picker;
            ValidateValue(p, p.Value);
            UpdateStringValue(p);
        }

        private static object ValueCoerced(DependencyObject d, object newValue)
        {
            return newValue;
        }

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Picker p = d as Picker;
            if (p == null)
            {
                return;
            }

            var res = ValidateValue(p, (int)e.NewValue);

            BindingExpression b = p.GetBindingExpression(ValueProperty);
            if (b != null && res) b.UpdateSource();
            if (res && p.UpdateStringValueMode == UpdateStringMode.Auto) UpdateStringValue(p);
        }

        private static bool ValidateValue(Picker p, int newValue)
        {
            var res = true;

            if (p.DataType == DisplayDataType.Integer)
            {
                p.StringValue = newValue.ToString();
            }
            else if (p.DataType == DisplayDataType.String)
            {
                if (p.StringCollection.Length < 1) { p.StringValue = string.Empty; }

                if (p.Value <= p.StringCollection.Length - 1 && p.Value > -1)
                {
                    p.StringValue = p.StringCollection[p.Value];
                }
                else
                {
                    p.StringValue = "<OutOfRange>";
                }
            }
            return res;
        }

        public static void UpdateStringValue(Picker p)
        {
            if (p.Formatting == "00" && p.Value < 0)
            {
                p.StringValue = string.Empty;
                return;
            }

            if (p.DataType == DisplayDataType.Integer)
            {
                if (p.Formatting != null)
                {
                    p.StringValue = p.Value.ToString(p.Formatting);
                }
                else
                {
                    p.StringValue = p.Value.ToString();
                }
            }
            else if (p.DataType == DisplayDataType.String)
            {
                if (p.Value <= p.StringCollection.Length - 1)
                {
                    p.StringValue = p.StringCollection[p.Value];
                }
                else
                {
                    p.StringValue = string.Empty;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            downButton = Template.FindName("PART_DownButton", this) as RepeatButton;
            upButton = Template.FindName("PART_UpButton", this) as RepeatButton;
            baseGrid = Template.FindName("PART_BaseGrid", this) as Grid;

            if (HideButtons)
            {
                downButton.Opacity = 0;
                upButton.Opacity = 0;
            }

            downButton.Click += downButton_Click;
            upButton.Click += upButton_Click;
            baseGrid.MouseEnter += baseGrid_MouseEnter;
            baseGrid.MouseLeave += baseGrid_MouseLeave;
            ValidateValue(this, Value);
            UpdateStringValue(this);

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

    public enum DisplayDataType
    {
        Integer,
        String
    }

    public enum UpdateStringMode
    {
        Auto,
        Explicit
    }
}
