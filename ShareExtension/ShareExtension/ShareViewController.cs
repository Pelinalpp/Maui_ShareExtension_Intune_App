using Newtonsoft.Json;
using Social;

namespace ShareExtension
{
    public partial class ShareViewController : SLComposeServiceViewController
    {
        private UILabel label;
        private UILabel itemListTitleLabel;
        private UIButton button;
        private UIScrollView scrollView;
        private UIStackView stackView;
        private UIButton shareButton;
        private UIColor neutralBlack;
        private UIButton cancelButton;

        List<string> PathList;
        int inputItemsLength;

        public ShareViewController(IntPtr handle) : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            try
            {
                neutralBlack = UIColor.FromRGB(64, 64, 65);

                var newController = new UIViewController();

                UIView headerView = new UIView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false
                };

                UILabel titleLabel = new UILabel
                {
                    Text = "Upload",

                    TextColor = neutralBlack,
                    Font = UIFont.BoldSystemFontOfSize(20),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };

                headerView.AddSubview(titleLabel);

                newController.View.AddSubview(headerView);

                scrollView = new UIScrollView();
                stackView = new UIStackView
                {
                    Axis = UILayoutConstraintAxis.Vertical,
                    Spacing = 12,
                    Distribution = UIStackViewDistribution.Fill,
                    Alignment = UIStackViewAlignment.Fill,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };

                scrollView.AddSubview(stackView);
                newController.View.AddSubview(scrollView);

                shareButton = UIButton.FromType(UIButtonType.System);
                shareButton.SetTitle("Upload", UIControlState.Normal);
                shareButton.BackgroundColor = UIColor.FromRGB(37, 172, 226); 
                shareButton.SetTitleColor(UIColor.White, UIControlState.Normal);
                shareButton.TitleLabel.TextAlignment = UITextAlignment.Center; 
                shareButton.Layer.CornerRadius = 8;
                shareButton.ClipsToBounds = true; 
                shareButton.TranslatesAutoresizingMaskIntoConstraints = false;
                shareButton.TouchUpInside += OnUploadButtonTouch;

                cancelButton = UIButton.FromType(UIButtonType.System);
                cancelButton.SetTitle("Cancel", UIControlState.Normal);
                cancelButton.BackgroundColor = UIColor.White; 
                cancelButton.SetTitleColor(neutralBlack, UIControlState.Normal);
                cancelButton.TitleLabel.TextAlignment = UITextAlignment.Center; 
                cancelButton.Layer.CornerRadius = 8;
                cancelButton.ClipsToBounds = true; 
                cancelButton.TranslatesAutoresizingMaskIntoConstraints = false;
                cancelButton.TouchUpInside += OnCancelButtonTouch;

                newController.View.AddSubview(shareButton);
                newController.View.AddSubview(cancelButton);


                scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
                stackView.TranslatesAutoresizingMaskIntoConstraints = false;
                float margin = 20;

                // New controller's view constraints
                NSLayoutConstraint.ActivateConstraints(new[]
                {
                    // Header view constraints
                    headerView.TopAnchor.ConstraintEqualTo(newController.View.TopAnchor, margin),
                    headerView.LeadingAnchor.ConstraintEqualTo(newController.View.LeadingAnchor, margin),
                    headerView.TrailingAnchor.ConstraintEqualTo(newController.View.TrailingAnchor, -margin),
                    headerView.HeightAnchor.ConstraintEqualTo(50),

                    // Title label constraints
                    titleLabel.CenterYAnchor.ConstraintEqualTo(headerView.CenterYAnchor),

                    // Scroll view constraints
                    scrollView.TopAnchor.ConstraintEqualTo(headerView.BottomAnchor, margin), // Start below header view

                    scrollView.LeadingAnchor.ConstraintEqualTo(newController.View.LeadingAnchor, margin),
                    scrollView.TrailingAnchor.ConstraintEqualTo(newController.View.TrailingAnchor, -margin),
                    scrollView.BottomAnchor.ConstraintEqualTo(shareButton.TopAnchor, -margin), 

                    // Stack view constraints
                    stackView.TopAnchor.ConstraintEqualTo(scrollView.TopAnchor),
                    stackView.BottomAnchor.ConstraintEqualTo(scrollView.BottomAnchor),
                    stackView.LeadingAnchor.ConstraintEqualTo(scrollView.LeadingAnchor),
                    stackView.TrailingAnchor.ConstraintEqualTo(scrollView.TrailingAnchor),
                    stackView.WidthAnchor.ConstraintEqualTo(scrollView.WidthAnchor),

                    // Share button constraints
                    shareButton.BottomAnchor.ConstraintEqualTo(newController.View.SafeAreaLayoutGuide.BottomAnchor, -20), // Increased margin from bottom
                    shareButton.TrailingAnchor.ConstraintEqualTo(newController.View.TrailingAnchor, -margin),
                    shareButton.WidthAnchor.ConstraintEqualTo(120), 
                    shareButton.HeightAnchor.ConstraintEqualTo(44),

                    // Cancel button constraints
                    cancelButton.BottomAnchor.ConstraintEqualTo(newController.View.SafeAreaLayoutGuide.BottomAnchor, -20), // Increased margin from bottom
                    cancelButton.LeadingAnchor.ConstraintEqualTo(newController.View.LeadingAnchor, margin),
                    cancelButton.WidthAnchor.ConstraintEqualTo(100),
                    cancelButton.HeightAnchor.ConstraintEqualTo(44),

                    // Align buttons horizontally
                    cancelButton.TrailingAnchor.ConstraintEqualTo(shareButton.LeadingAnchor, -10),
                });

                NSExtensionItem item = ExtensionContext.InputItems[0];
                inputItemsLength = ExtensionContext.InputItems[0].Attachments.Length;

                string message = "Processing selected " + inputItemsLength.ToString() + " item(s) to contunie upload operation. Please wait...";

                if (item.Attachments.Length <= 50)
                {
                    PathList = new List<string>();

                    var FileManager = new NSFileManager();


                    var appGroupContainer = FileManager.GetContainerUrl("group.com.mauiapp");
                    if (appGroupContainer == null)
                    {
                        //Logger.Log(Logger.LogLevel.Error,"Error: App group container URL is null");
                    }

                    var appGroupContainerPath = appGroupContainer.Path;

                    try
                    {
                        var directoryContents = Directory.GetFiles(appGroupContainerPath);
                        foreach (var filePath in directoryContents)
                        {
                            File.Delete(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error while cleaning up directory: " + ex.Message);
                    }

                    foreach (NSItemProvider itemProvider in item.Attachments)
                    {
                        var type = itemProvider.Description.Split('"');
                        if (itemProvider.HasItemConformingTo(type[1]))
                        {
                            itemProvider.LoadItem(type[1], null, (url, error) =>
                            {
                                if (url != null)
                                {
                                    var newUrl = ((NSUrl)(url));

                                    var destFilePath = Path.Combine(appGroupContainerPath, Path.GetFileName(newUrl.Path));

                                    int i = 0;
                                    while (true)
                                    {
                                        if (PathList.Any(x => x == destFilePath))
                                        {
                                            var tempDestPath = Path.Combine(appGroupContainerPath, Path.GetFileName(newUrl.Path));
                                            destFilePath = Path.Combine(
                                                  Path.GetDirectoryName(tempDestPath),
                                                  Path.GetFileNameWithoutExtension(tempDestPath) + "(" + i.ToString() + ")" +
                                                  Path.GetExtension(tempDestPath));
                                            i++;
                                        }
                                        else
                                            break;
                                    }

                                    NSError err = new NSError();
                                    FileManager.Copy(srcPath: newUrl.Path, dstPath: destFilePath, error: out err);

                                    PathList.Add(destFilePath);

                                    if (PathList.Count == inputItemsLength)
                                    {
                                        try
                                        {
                                            InvokeOnMainThread(() =>
                                          {
                                              DisplayFileList();
                                          });

                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                            });
                        }
                    }
                }

                NavigationController.NavigationBar.TopItem.RightBarButtonItem = null;
                NavigationController.NavigationBar.TopItem.LeftBarButtonItem = null;
                itemListTitleLabel = new UILabel
                {
                    Text = $"Items:",
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.SystemFontOfSize(16),
                    Lines = 0, // Allows multiple lines
                    LineBreakMode = UILineBreakMode.WordWrap,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                stackView.AddArrangedSubview(itemListTitleLabel);

                View.Hidden = true;

                stackView.SetNeedsLayout();
                stackView.LayoutIfNeeded();
                scrollView.ContentSize = stackView.Frame.Size;

                newController.View.BackgroundColor = UIColor.White;
                this.PresentModalViewController(newController, false);

            }
            catch (Exception ex)
            {
                var exceptionDetails = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    ExceptionType = ex.GetType().FullName,
                    DateTime = DateTime.Now
                };

                string exceptionJson = JsonConvert.SerializeObject(exceptionDetails, Formatting.Indented);
                ShowToastMessage(exceptionJson);
                Logger.Log(Logger.LogLevel.Error, exceptionJson);
            }
        }


        private void ShowToastMessage(string message)
        {
            var toastLabel = new UILabel
            {
                Text = message,
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Black.ColorWithAlpha(0.7f),
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.BoldSystemFontOfSize(14),
                Lines = 0, 
                Alpha = 0, 
                Layer = { CornerRadius = 8 },
                ClipsToBounds = true,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            var mainWindow = UIApplication.SharedApplication.KeyWindow;
            if (mainWindow == null) return;

            mainWindow.AddSubview(toastLabel);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                toastLabel.LeadingAnchor.ConstraintEqualTo(mainWindow.LeadingAnchor, 20),
                toastLabel.TrailingAnchor.ConstraintEqualTo(mainWindow.TrailingAnchor, -20),
                toastLabel.BottomAnchor.ConstraintEqualTo(mainWindow.BottomAnchor, -100),
                toastLabel.HeightAnchor.ConstraintGreaterThanOrEqualTo(40)
            });

            UIView.Animate(0.5, () =>
            {
                toastLabel.Alpha = 1; 
            }, () =>
            {
                UIView.Animate(2.5, () =>
                {
                    toastLabel.Alpha = 0; 
                }, () =>
                {
                    toastLabel.RemoveFromSuperview(); 
                });
            });
        }

       public override UITextView TextView
       {
            get
            {
                UITextView textView = base.TextView;
                textView.Delegate = this;
                textView.ResignFirstResponder(); // Set delegate
                return textView;
            }
       }

        public bool ShouldChangeText(UITextView textView, NSRange range, string text)
        {
            return false; // Prevent changes
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.TextView.Editable=false;
        }

        public override void PresentationAnimationDidFinish()
        {
            base.PresentationAnimationDidFinish();
        }

        public override bool CanBecomeFirstResponder => false;
        private void OnUploadButtonTouch(object sender, EventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                CallTheMauiApp(this);
            });
        }

        private void OnCancelButtonTouch(object sender, EventArgs e)
        {
            // Handle cancel action here
            ExtensionContext.CancelRequest(null);
        }

        private void CallTheMauiApp(ShareViewController shareViewController)
        {
            try
            {
                if (PathList != null && PathList.Count == inputItemsLength)
                {
                    string pathList = "";
                    foreach (var path in PathList)
                    {
                        pathList += path + "&";
                    }
                    pathList = pathList.Substring(0, pathList.Length - 1);
                    PathList = new List<string>();
                    string customUrlScheme = $"com.mauiapp://ShareExtension?{pathList}";
                    var customUrl = new NSUrl(customUrlScheme);

                    UIApplication.SharedApplication.OpenUrl(customUrl, new NSDictionary(), success =>
                    {
                        if (success)
                        {
                            Console.WriteLine("Maui App app opened successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to open Maui App app.");
                        }
                    });

                    ExtensionContext?.CompleteRequest(new NSExtensionItem[0], null);
                }
            }
            catch (Exception ex)
            {
                string exp = ex.Message;
            }
        }
 
        private void DisplayFileList()
        {
            foreach (var filePath in PathList)
            {
                string fileName = Path.GetFileName(filePath);
                UILabel fileLabel = new UILabel
                {
                    Text = fileName,
                    TextColor = UIColor.FromRGB(160, 160, 160),
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.SystemFontOfSize(14),
                    Lines = 0, // Allows multiple lines
                    LineBreakMode = UILineBreakMode.WordWrap,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                stackView.AddArrangedSubview(fileLabel);

                UIView separator = new UIView
                {
                    BackgroundColor = UIColor.FromRGB(236, 237, 239),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                stackView.AddArrangedSubview(separator);

                separator.HeightAnchor.ConstraintEqualTo(1).Active = true;
            }
        }

        public override bool IsContentValid()
        {
            return true;
        }

        public override void DidSelectPost()
        {
            // This is called after the user selects Post. Do the upload of contentText and/or NSExtensionContext attachments.
            // Inform the host that we're done, so it un-blocks its UI. Note: Alternatively you could call super's -didSelectPost, which will similarly complete the extension context.
            ExtensionContext.CompleteRequest(new NSExtensionItem[0], null);
        }

        public override SLComposeSheetConfigurationItem[] GetConfigurationItems()
        {
            // To add configuration options via table cells at the bottom of the sheet, return an array of SLComposeSheetConfigurationItem here.
            return new SLComposeSheetConfigurationItem[0];
        }
    }
}
