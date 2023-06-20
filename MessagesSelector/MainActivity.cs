using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using MessagesSelector.Items;
using MessagesSelector.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Message = MessagesSelector.Items.Message;

namespace MessagesSelector
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            searchButton = FindViewById<Android.Widget.Button>(Resource.Id.search_button);
            searchButton.Click += Button1_Click;

            searchTextBox = FindViewById<Android.Widget.EditText>(Resource.Id.search_text_box);

            messagesListView = FindViewById<Android.Widget.ListView>(Resource.Id.messages_list);
            messagesListView.ItemClick += (s, e) => {
                Android.Widget.Toast.MakeText(this, messagesObservableCollection[e.Position].Text, Android.Widget.ToastLength.Long).Show();
            };

            contactsAndSMSManager = new ContactsAndSMSManager();

            #region pre load

            ActivityCompat.RequestPermissions(this, new String[] {
                    Manifest.Permission.ReadSms,
                    Manifest.Permission.ReadCallLog,
                    Manifest.Permission.ReadContacts
                    }, 1);
            contactsAndSMSManager.GetAllContactsAsync();

            #endregion

        }

        Android.Widget.Button searchButton;

        Android.Widget.EditText searchTextBox;

        Android.Widget.ListView messagesListView;

        // ---------------------------------------- //
        private ContactsAndSMSManager contactsAndSMSManager;

        private ObservableCollection<Message> messagesObservableCollection;

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //sort and filtr
            messagesObservableCollection = contactsAndSMSManager.GetMessagesByFiltr(this, searchTextBox.Text);
            if (messagesObservableCollection != null)
            {
                List<string> strings = new List<string>();
                foreach(var m in messagesObservableCollection)
                {
                    if(m.Person != null)
                        strings.Add("[" + m.Person + "] : " + m.Text);
                    else
                        strings.Add("[" + m.Address + "] : " + m.Text);
                }
                messagesListView.Adapter = new Android.Widget.ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, strings);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
