using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MAProjektvorlageGrid.Common
{
    /// <summary>
    /// NavigationManager aids in navigation between pages.  It provides commands used to 
    /// navigate back and forward as well as registers for standard mouse and keyboard 
    /// shortcuts used to go back and forward.  In addition it integrates SuspensionManger
    /// to handle process lifetime management and state management when navigating between
    /// pages.
    /// </summary>
    /// <example>
    /// To make use of NavigationManager, follow these two steps or
    /// start with a BasicPage or any other Page item template other than BlankPage.
    /// 
    /// 1) Create an instance of the NaivgationHelper somewhere such as in the 
    ///     constructor for the page and register a callback for the LoadState and 
    ///     SaveState events.
    /// <code>
    ///     public MyPage()
    ///     {
    ///         this.InitializeComponent();
    ///         var navigationHelper = new NavigationHelper(this);
    ///         this.navigationHelper.LoadState += navigationHelper_LoadState;
    ///         this.navigationHelper.SaveState += navigationHelper_SaveState;
    ///     }
    ///     
    ///     private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
    ///     { }
    ///     private async void navigationHelper_SaveState(object sender, LoadStateEventArgs e)
    ///     { }
    /// </code>
    /// 
    /// 2) Register the page to call into the NavigationManager whenever the page participates 
    ///     in navigation by overriding the <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedTo"/> 
    ///     and <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedFrom"/> events.
    /// <code>
    ///     protected override void OnNavigatedTo(NavigationEventArgs e)
    ///     {
    ///         navigationHelper.OnNavigatedTo(e);
    ///     }
    ///     
    ///     protected override void OnNavigatedFrom(NavigationEventArgs e)
    ///     {
    ///         navigationHelper.OnNavigatedFrom(e);
    ///     }
    /// </code>
    /// </example>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class NavigationHelper : DependencyObject
    {
        private Page Page { get; set; }
        private Frame Frame { get { return this.Page.Frame; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationHelper"/> class.
        /// </summary>
        /// <param name="page">A reference to the current page used for navigation.  
        /// This reference allows for frame manipulation and to ensure that keyboard 
        /// navigation requests only occur when the page is occupying the entire window.</param>
        public NavigationHelper(Page page)
        {
            this.Page = page;

            // Zwei �nderungen vornehmen, wenn diese Seite Teil der visuellen Struktur ist:
            // 1) Den Ansichtszustand der Anwendung dem visuellen Zustand f�r die Seite zuordnen
            // 2) Tastatur- und Mausnavigationsanforderungen bearbeiten
            this.Page.Loaded += (sender, e) =>
            {
                // Tastatur- und Mausnavigation trifft nur zu, wenn das gesamte Fenster ausgef�llt wird.
                if (this.Page.ActualHeight == Window.Current.Bounds.Height &&
                    this.Page.ActualWidth == Window.Current.Bounds.Width)
                {
                    // Direkt am Fenster lauschen, sodass kein Fokus erforderlich ist
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed +=
                        this.CoreWindow_PointerPressed;
                }
            };

            // Dieselben �nderungen r�ckg�ngig machen, wenn die Seite nicht mehr sichtbar ist
            this.Page.Unloaded += (sender, e) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                    CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -=
                    this.CoreWindow_PointerPressed;
            };
        }

        #region Navigationsunterst�tzung dient.

        RelayCommand _goBackCommand;
        RelayCommand _goForwardCommand;

        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to the back Button's Command property
        /// for navigating to the most recent item in back navigation history, if a Frame
        /// manages its own navigation history.
        /// 
        /// The <see cref="RelayCommand"/> is set up to use the virtual method <see cref="GoBack"/>
        /// as the Execute Action and <see cref="CanGoBack"/> for CanExecute.
        /// </summary>
        public RelayCommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(
                        () => this.GoBack(),
                        () => this.CanGoBack());
                }
                return _goBackCommand;
            }
            set
            {
                _goBackCommand = value;
            }
        }
        /// <summary>
        /// <see cref="RelayCommand"/> used for navigating to the most recent item in 
        /// the forward navigation history, if a Frame manages its own navigation history.
        /// 
        /// The <see cref="RelayCommand"/> is set up to use the virtual method <see cref="GoForward"/>
        /// as the Execute Action and <see cref="CanGoForward"/> for CanExecute.
        /// </summary>
        public RelayCommand GoForwardCommand
        {
            get
            {
                if (_goForwardCommand == null)
                {
                    _goForwardCommand = new RelayCommand(
                        () => this.GoForward(),
                        () => this.CanGoForward());
                }
                return _goForwardCommand;
            }
        }

        /// <summary>
        /// Virtual method used by the <see cref="GoBackCommand"/> property
        /// to determine if the <see cref="Frame"/> can go back.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Frame"/> has at least one entry 
        /// in the back navigation history.
        /// </returns>
        public virtual bool CanGoBack()
        {
            return this.Frame != null && this.Frame.CanGoBack;
        }
        /// <summary>
        /// Virtual method used by the <see cref="GoForwardCommand"/> property
        /// to determine if the <see cref="Frame"/> can go forward.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Frame"/> has at least one entry 
        /// in the forward navigation history.
        /// </returns>
        public virtual bool CanGoForward()
        {
            return this.Frame != null && this.Frame.CanGoForward;
        }

        /// <summary>
        /// Virtual method used by the <see cref="GoBackCommand"/> property
        /// to invoke the <see cref="Windows.UI.Xaml.Controls.Frame.GoBack"/> method.
        /// </summary>
        public virtual void GoBack()
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }
        /// <summary>
        /// Virtual method used by the <see cref="GoForwardCommand"/> property
        /// to invoke the <see cref="Windows.UI.Xaml.Controls.Frame.GoForward"/> method.
        /// </summary>
        public virtual void GoForward()
        {
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
        }

        /// <summary>
        /// Wird bei jeder Tastatureingabe aufgerufen, einschlie�lich Systemtasten wie ALT-Tastenkombinationen, wenn
        /// diese Seite aktiv ist und das gesamte Fenster ausf�llt.  Wird zum Erkennen von Tastaturnavigation verwendet
        /// zwischen Seiten, auch wenn die Seite selbst nicht den Fokus hat.
        /// </summary>
        /// <param name="sender">Instanz, von der das Ereignis ausgel�st wurde.</param>
        /// <param name="e">Ereignisdaten, die die Bedingungen beschreiben, die zu dem Ereignis gef�hrt haben.</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
            AcceleratorKeyEventArgs e)
        {
            var virtualKey = e.VirtualKey;

            // Weitere Untersuchungen nur durchf�hren, wenn die Taste "Nach links", "Nach rechts" oder die dezidierten Tasten "Zur�ck" oder "Weiter"
            // gedr�ckt werden
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                e.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // Wenn die Taste "Zur�ck" oder ALT+NACH-LINKS-TASTE gedr�ckt wird, zur�ck navigieren
                    e.Handled = true;
                    this.GoBackCommand.Execute(null);
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // Wenn die Taste "Weiter" oder ALT+NACH-RECHTS-TASTE gedr�ckt wird, vorw�rts navigieren
                    e.Handled = true;
                    this.GoForwardCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Wird bei jedem Mausklick, jeder Touchscreenber�hrung oder einer �quivalenten Interaktion aufgerufen, wenn diese
        /// Seite aktiv ist und das gesamte Fenster ausf�llt.  Wird zum Erkennen von "Weiter"- und "Zur�ck"-Maustastenklicks
        /// im Browserstil verwendet, um zwischen Seiten zu navigieren.
        /// </summary>
        /// <param name="sender">Instanz, von der das Ereignis ausgel�st wurde.</param>
        /// <param name="e">Ereignisdaten, die die Bedingungen beschreiben, die zu dem Ereignis gef�hrt haben.</param>
        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // Tastenkombinationen mit der linken, rechten und mittleren Taste ignorieren
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // Wenn "Zur�ck" oder "Vorw�rts" gedr�ckt wird (jedoch nicht gleichzeitig), entsprechend navigieren
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed) this.GoBackCommand.Execute(null);
                if (forwardPressed) this.GoForwardCommand.Execute(null);
            }
        }

        #endregion

        #region Verwaltung der Prozesslebensdauer

        private String _pageKey;

        /// <summary>
        /// Register this event on the current page to populate the page
        /// with content passed during navigation as well as any saved
        /// state provided when recreating a page from a prior session.
        /// </summary>
        public event LoadStateEventHandler LoadState;
        /// <summary>
        /// Register this event on the current page to preserve
        /// state associated with the current page in case the
        /// application is suspended or the page is discarded from
        /// the navigaqtion cache.
        /// </summary>
        public event SaveStateEventHandler SaveState;

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.  
        /// This method calls <see cref="LoadState"/>, where all page specific
        /// navigation and process lifetime management logic should be placed.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.  Die
        /// Parametereigenschaft stellt die anzuzeigende Gruppe bereit.</param>
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            this._pageKey = "Page-" + this.Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Bestehenden Zustand f�r die Vorw�rtsnavigation l�schen, wenn dem Navigationsstapel eine neue
                // Seite hinzugef�gt wird
                var nextPageKey = this._pageKey;
                int nextPageIndex = this.Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                // Den Navigationsparameter an die neue Seite �bergeben
                if (this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, null));
                }
            }
            else
            {
                // Den Navigationsparameter und den beibehaltenen Seitenzustand an die Seite �bergeben,
                // dabei die gleiche Strategie verwenden wie zum Laden des angehaltenen Zustands und zum erneuten Erstellen von im Cache verworfenen
                // Seiten
                if (this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, (Dictionary<String, Object>)frameState[this._pageKey]));
                }
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite nicht mehr in einem Rahmen angezeigt wird.
        /// This method calls <see cref="SaveState"/>, where all page specific
        /// navigation and process lifetime management logic should be placed.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.  Die
        /// Parametereigenschaft stellt die anzuzeigende Gruppe bereit.</param>
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            var pageState = new Dictionary<String, Object>();
            if (this.SaveState != null)
            {
                this.SaveState(this, new SaveStateEventArgs(pageState));
            }
            frameState[_pageKey] = pageState;
        }

        #endregion
    }

    /// <summary>
    /// Represents the method that will handle the <see cref="NavigationHelper.LoadState"/>event
    /// </summary>
    public delegate void LoadStateEventHandler(object sender, LoadStateEventArgs e);
    /// <summary>
    /// Represents the method that will handle the <see cref="NavigationHelper.SaveState"/>event
    /// </summary>
    public delegate void SaveStateEventHandler(object sender, SaveStateEventArgs e);

    /// <summary>
    /// Class used to hold the event data required when a page attempts to load state.
    /// </summary>
    public class LoadStateEventArgs : EventArgs
    {
        /// <summary>
        /// The parameter value passed to <see cref="Frame.Navigate(Type, Object)"/> 
        /// when this page was initially requested.
        /// </summary>
        public Object NavigationParameter { get; private set; }
        /// <summary>
        /// A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.
        /// </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadStateEventArgs"/> class.
        /// </summary>
        /// <param name="navigationParameter">
        /// The parameter value passed to <see cref="Frame.Navigate(Type, Object)"/> 
        /// when this page was initially requested.
        /// </param>
        /// <param name="pageState">
        /// A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.
        /// </param>
        public LoadStateEventArgs(Object navigationParameter, Dictionary<string, Object> pageState)
            : base()
        {
            this.NavigationParameter = navigationParameter;
            this.PageState = pageState;
        }
    }
    /// <summary>
    /// Class used to hold the event data required when a page attempts to save state.
    /// </summary>
    public class SaveStateEventArgs : EventArgs
    {
        /// <summary>
        /// An empty dictionary to be populated with serializable state.
        /// </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveStateEventArgs"/> class.
        /// </summary>
        /// <param name="pageState">Ein leeres W�rterbuch, das mit dem serialisierbaren Zustand aufgef�llt wird.</param>
        public SaveStateEventArgs(Dictionary<string, Object> pageState)
            : base()
        {
            this.PageState = pageState;
        }
    }
}
