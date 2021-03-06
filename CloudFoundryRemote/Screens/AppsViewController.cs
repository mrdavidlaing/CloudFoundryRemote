using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using Mono.CFoundry.Models;
using CloudFoundryRemote.Helpers.Tables;
using CloudFoundryRemote.Helpers;

namespace CloudFoundryRemote
{
	public partial class AppsViewController : UIViewController
	{
		UITableViewSource _tblSource;
		Mono.CFoundry.Client _client;

		public AppsViewController (Mono.CFoundry.Client client, List<App> apps) : base ("AppsViewController", null)
		{
			this.Title = "Applications";

			_client = client;
			_tblSource = AppsAsTableView(apps);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationItem.RightBarButtonItem = VisualHelper.NewLogoutButton (NavigationController);

			UITableView tblApps = new UITableView (new RectangleF (0, 0, View.Frame.Width, View.Frame.Height), UITableViewStyle.Plain);
			Add (tblApps);

			tblApps.Source = _tblSource;
		}

		private UITableViewSource AppsAsTableView(List<App> apps)
		{
			List<AppTableSourceItem> source = new List<AppTableSourceItem> ();

			foreach (var app in apps)

				source.Add (new AppTableSourceItem(){
					Guid = app.Guid,
					Caption = app.Name,
					App = app,
					RowClick = (sender, e) => {
						var args = (AppRowEventArgs)e;

						UIView pleaseWait = null;

						pleaseWait = VisualHelper.ShowPleaseWait("Loading...", View, () => {

							AppDetailViewController appDetailViewController = 
								new AppDetailViewController(_client, args.Item.App.Guid);

							if (pleaseWait != null)
								VisualHelper.HidePleaseWait(pleaseWait, () => {
									pleaseWait.RemoveFromSuperview ();
									this.NavigationController.PushViewController(appDetailViewController, true);
								});
						});
					}
				});

			return new AppTableSource (source.ToArray ());
		}
	}
}

