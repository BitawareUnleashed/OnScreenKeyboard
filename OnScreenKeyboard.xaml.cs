/**
    Copyright(c) Microsoft Open Technologies, Inc.All rights reserved.
   The MIT License(MIT)
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files(the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions :
    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
**/

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OnScreenKeyboard
{
    public class ContentBuffer
    {
        public ContentBuffer() { }

        private string _str = null;
        private int _selectionStart = 0;
        private int _selectionLength = 0;

        public int SelectionStart
        {
            get
            {
                return _selectionStart;
            }
            set
            {
                _selectionStart = value;
            }
        }
        public int SelectionLength
        {
            get
            {
                return _selectionLength;
            }
            set
            {
                _selectionLength = value;
            }
        }
        public string Content
        {
            get
            {
                return _str;
            }
            set
            {
                _str = value;
            }
        }
    }
    public partial class OnScreenKeyBoard : UserControl
    {
        private static ContentBuffer _buffer;
        public static ContentBuffer Buffer
        {
            get
            {
                if (_buffer == null)
                {
                    _buffer = new ContentBuffer();
                }
                return _buffer;
            }
            private set
            {
                if (_buffer == null)
                {
                    _buffer = new ContentBuffer();
                }
                _buffer = value;
                
            }
        }

        public static readonly DependencyProperty OutputStringProperty = DependencyProperty.Register("BufferString", typeof(string), typeof(OnScreenKeyBoard), new PropertyMetadata(string.Empty, OnTextBoxTextPropertyChanged));
        public string BufferString
        {
            get
            {
                return (string)GetValue(OutputStringProperty);
            }
            set
            {
                SetValue(OutputStringProperty, value);
                AssociatedControl.Text = Buffer.Content;
            }
        }

        private static void OnTextBoxTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Sync up the string buffer context.
            Buffer.Content = (string)e.NewValue;
        }

        public TextBox AssociatedControl { get; set; }

        public void RegisterEditControl(Control control)
        {
            if (AssociatedControl != null)
                if (AssociatedControl.Name.Length > 0 && AssociatedControl.Name == (control as TextBox).Name)
                    return;

            UnRegisterEditControl(control);

            AssociatedControl = control as TextBox;
            AssociatedControl.SelectionChanged += Target_SelectionChanged;
            AssociatedControl.GotFocus += Target_GotFocus;

            Buffer = new ContentBuffer();
            Buffer.Content = AssociatedControl.Text;
            BufferString = AssociatedControl.Text;
        }



        public void UnRegisterEditControl(Control control)
        {
            if (AssociatedControl == null)
                return;

            AssociatedControl = control as TextBox;
            AssociatedControl.SelectionChanged -= Target_SelectionChanged;
            AssociatedControl.GotFocus -= Target_GotFocus;
            AssociatedControl = null;
        }

        private void Target_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AssociatedControl = sender as TextBox;
            if (AssociatedControl.FocusState == FocusState.Pointer)
            {
                Buffer.SelectionStart = AssociatedControl.SelectionStart;
                Buffer.SelectionLength = AssociatedControl.SelectionLength;
            }
        }

        private void Target_GotFocus(object sender, RoutedEventArgs e)
        {
            AssociatedControl = sender as TextBox;
            if (AssociatedControl.FocusState == FocusState.Pointer)
            {
                Buffer.Content = AssociatedControl.Text;
                Buffer.SelectionStart = AssociatedControl.SelectionStart;
                Buffer.SelectionLength = AssociatedControl.SelectionLength;
            }
        }

        public OnScreenKeyBoard()
        {
            DataContext = new KeyboardViewModel(this);
            InitializeComponent();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}

