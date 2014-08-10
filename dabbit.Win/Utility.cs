using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using System.Collections.Specialized;

namespace dabbit.Base
{

    public static class StringUtility
    {
        public static string ToMd5(this string me)
        {
            byte[] bytePassword = Encoding.UTF8.GetBytes(me ?? "20412333");

            using (MD5 md5 = MD5.Create())
            {
                byte[] byteHashedPassword = md5.ComputeHash(bytePassword);
                return BitConverter.ToString(byteHashedPassword).Replace("-", "");
            }
        }

        public static void InvokeIfRequired(this System.Windows.UIElement control, Action action)
        {
            if (!control.Dispatcher.CheckAccess())
            {
                try
                {
                    control.Dispatcher.Invoke(action);
                }
                catch (TaskCanceledException)
                { } // TODO: Handle this better!
            }
            else
            {
                action();
            }
        }

        public static Paragraph IrcStringToRtf(this string input, SolidColorBrush defaultFontColor)
        {
            return input.IrcStringToRtf(new SolidColorBrush[] { Brushes.White, Brushes.Black, Brushes.Blue, Brushes.Green, 
                Brushes.Red, Brushes.Brown, Brushes.Purple, Brushes.Orange, Brushes.Yellow, Brushes.Lime, Brushes.Teal,
                Brushes.Aqua, Brushes.RoyalBlue, Brushes.Fuchsia, Brushes.Gray, Brushes.Silver }, defaultFontColor);
        }

        private static readonly Regex rgx = new Regex("[A-z]{2,4}://[^ ]+");

        /// <summary>
        /// Takes a string of IRC formatted input and converts to Xaml style RTF. 
        /// Converts links like http://, https:// ftp://, wc://, etc to a clickable 
        /// link. 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ircToHexColors"></param>
        /// <returns></returns>
        public static Paragraph IrcStringToRtf(this string input, SolidColorBrush[] ircToHexColors, SolidColorBrush defaultFontColor)
        {
            if (ircToHexColors.Length < 15)
                throw new ArgumentException("Not enough colors in ircToHexColors");
            
            Paragraph result = new Paragraph();

            StringBuilder normal = new StringBuilder();

            double calcNumber = 0;

            bool inColor = false;
            bool inForeground = true;
            bool bold = false;
            bool italics = false;
            bool underline = false;
            bool strikethrough = false;
            bool overline = false;
            SolidColorBrush foregroundColor = defaultFontColor;
            SolidColorBrush backgroundColor = Brushes.Transparent;


            var parts = Regex.Split(input, rgx.ToString());
            var urls = rgx.Matches(input);

            for (int i = 0; i < parts.Length; i++)
            {
                string piece = parts[i];

                //foreach(char c in piece)
                for(int j =0; j < piece.Length; j++)
                {
                    char c = piece[j];

                     if (c == (char)ControlCode.Color)
                     {
                         // Make sure the next character isn't a # indicating we aren't changing the color
                         if (j != piece.Length - 1 && ! piece[j+1].IsDigit())
                         {
                             result.Inlines.Add(GenerateRun(normal.ToString(), foregroundColor, backgroundColor, bold, italics, strikethrough, underline, overline));
                             normal.Clear();

                             foregroundColor = defaultFontColor;
                             backgroundColor = Brushes.Transparent;
                             inColor = false;
                         }
                         else
                         {
                             // If we've already set the color, push that so when we change it, we don't overwrite it
                             if (inColor && normal.Length > 0)
                             {
                                 result.Inlines.Add(GenerateRun(normal.ToString(), foregroundColor, backgroundColor, bold, italics, strikethrough, underline, overline));
                                 normal.Clear();
                             }

                             inColor = true;
                         }
                     }
                     else if (c == (char) ControlCode.Bold)
                     {
                         result.Inlines.Add(GenerateRun(normal.ToString(), foregroundColor, backgroundColor, bold, italics, strikethrough, underline, overline));
                         normal.Clear();
                         bold = !bold;
                     }
                     else if (c == (char)ControlCode.Italic)
                     {
                         result.Inlines.Add(GenerateRun(normal.ToString(), foregroundColor, backgroundColor, bold, italics, strikethrough, underline, overline));
                         normal.Clear();
                         italics = !italics;

                     }
                     else if (c == (char)ControlCode.Underline)
                     {
                         result.Inlines.Add(GenerateRun(normal.ToString(), foregroundColor, backgroundColor, bold, italics, strikethrough, underline, overline));
                         normal.Clear();
                         underline = !underline;

                     }
                     else if (c == (char)ControlCode.StrikeThrough)
                     {
                         result.Inlines.Add(GenerateRun(normal.ToString(), foregroundColor, backgroundColor, bold, italics, strikethrough, underline, overline));
                         normal.Clear();
                         strikethrough = !strikethrough;

                     }
                     else if (c == (char)ControlCode.Reverse)
                     {
                         result.Inlines.Add(GenerateRun(normal.ToString(), foregroundColor, backgroundColor, bold, italics, strikethrough, underline, overline));
                         normal.Clear();

                         SolidColorBrush tmp = foregroundColor;
                         foregroundColor = (backgroundColor != Brushes.Transparent ? backgroundColor : Brushes.White);
                         backgroundColor = tmp;
                     }
                     else if (c== (char)ControlCode.Underline2)
                     {
                         result.Inlines.Add(GenerateRun(normal.ToString(), foregroundColor, backgroundColor, bold, italics, strikethrough, underline, overline));
                         normal.Clear();
                         overline = !overline;
                     }
                     else if (c == (char)ControlCode.Reset)
                     {
                         result.Inlines.Add(GenerateRun(normal.ToString(), foregroundColor, backgroundColor, bold, italics, strikethrough, underline, overline));
                         normal.Clear();
                         overline = underline = bold = italics = bold = strikethrough = false;
                         foregroundColor = defaultFontColor;
                         backgroundColor = Brushes.Transparent;
                     }
                     else if (inColor && c.IsDigit())
                     {
                         calcNumber = (calcNumber * 10) +  char.GetNumericValue(c);
                     }
                     else if (inColor && ! c.IsDigit() && c == ',')
                     {
                         inForeground = false;
                         foregroundColor = ircToHexColors[(int)calcNumber];
                         calcNumber = 0;
                     }
                     else if (inColor && ! c.IsDigit() && ! inForeground)
                     {
                         inForeground = true;
                         backgroundColor = ircToHexColors[(int)calcNumber];
                         calcNumber = 0;
                         normal.Append(c);
                     }
                     else if (inColor && ! c.IsDigit() && calcNumber != 0)
                     {
                         foregroundColor = ircToHexColors[(int)calcNumber];
                         
                         calcNumber = 0;
                         normal.Append(c);
                     }
                     else
                     {
                         normal.Append(c);
                     }
                }

                // If we just switched to a color, place normal text into the result
                if (normal.Length > 0)
                {
                    result.Inlines.Add(GenerateRun(normal.ToString(), foregroundColor, backgroundColor, bold, italics, strikethrough, underline, overline));
                    normal.Clear();
                }

                // Make sure we have other URLs that take place after this line.
                if (urls.Count - 1 >= i)
                {
                    // Create a link with an open to browser action.
                    var link = new Hyperlink() { NavigateUri = new Uri(urls[i].Groups[0].ToString()) };
                    link.Inlines.Add(new Run(urls[i].Groups[0].ToString()));

                    link.Foreground = (foregroundColor != defaultFontColor ? foregroundColor : link.Foreground);
                    link.Background = backgroundColor;

                    link.RequestNavigate += (object sender, System.Windows.Navigation.RequestNavigateEventArgs e) =>
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
                        e.Handled = true;
                    };

                    result.Inlines.Add(link);
                }
            }

            // \003#,[#]\003
            // \015 clears all bold, italics, colors, underline, etc




            return result;
        }

        private static Run GenerateRun(string text, SolidColorBrush foreground, SolidColorBrush background, bool bold, bool italics, bool strike, bool underline, bool overline)
        {
            var run = new Run(text) { Foreground = foreground, Background = background };

            if (bold)
                run.FontWeight = FontWeights.Bold;

            if (italics)
                run.FontStyle = FontStyles.Italic;

            if (strike)
                run.TextDecorations.Add(TextDecorations.Strikethrough);

            if (underline)
                run.TextDecorations.Add(TextDecorations.Underline);

            if (overline)
                run.TextDecorations.Add(TextDecorations.OverLine);

            return run;
        }

    }

    public class ObservableStack<T> : Stack<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public ObservableStack()
        {
        }

        public ObservableStack(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                base.Push(item);
        }

        public ObservableStack(List<T> list)
        {
            foreach (var item in list)
                base.Push(item);
        }


        public new virtual void Clear()
        {
            base.Clear();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new virtual T Pop()
        {
            var item = base.Pop();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return item;
        }

        public new virtual void Push(T item)
        {
            base.Push(item);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }


        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;


        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.RaiseCollectionChanged(e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e);
        }


        protected virtual event PropertyChangedEventHandler PropertyChanged;


        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, e);
        }

        private void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }


        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.PropertyChanged += value; }
            remove { this.PropertyChanged -= value; }
        }
    }
}
