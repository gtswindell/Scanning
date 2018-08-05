using Microsoft.Windows.Themes;
using System;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

using TWAINComm.Demo.ViewModels;

namespace TWAINComm.Demo.Behaviors
{
    public class ScrollToBottom : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            if ( this.AssociatedObject == null )
            {
                throw new ArgumentNullException( "AssociatedObject" );
            }

            Grid parentGrid = VisualTreeHelper.GetParent( this.AssociatedObject ) as Grid;
            if ( parentGrid != null )
            {
                ParentVM = parentGrid.DataContext as MainWindowViewModel;
                if ( ParentVM != null )
                {
                    ParentVM.ScrollToBottom += ParentVM_ScrollToBottom;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if ( ParentVM != null )
            {
                ParentVM.ScrollToBottom -= ParentVM_ScrollToBottom;
            }
        }

        private void ParentVM_ScrollToBottom()
        {
            ScrollViewer scrollViewer = GetScrollViewer( this.AssociatedObject );
            if ( scrollViewer != null )
            {
                scrollViewer.ScrollToBottom();
            }
        }

        private ScrollViewer GetScrollViewer( TextBox textBox )
        {
            ScrollViewer ret = null;

            ListBoxChrome listBox = null;
            if ( textBox != null && VisualTreeHelper.GetChildrenCount( textBox ) > 0 )
            {
                listBox = VisualTreeHelper.GetChild( textBox, 0 ) as ListBoxChrome;
            }

            if ( listBox != null && VisualTreeHelper.GetChildrenCount( listBox ) > 0 )
            {
                ret = VisualTreeHelper.GetChild( listBox, 0 ) as ScrollViewer;
            }

            return ret;
        }

        private MainWindowViewModel parentVM = null;
        private MainWindowViewModel ParentVM
        {
            get { return parentVM; }
            set { parentVM = value; }
        }
    }
}
