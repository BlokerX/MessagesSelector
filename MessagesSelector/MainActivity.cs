﻿using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Telephony;
using MessagesSelector.Services;
using AndroidX.Core.App;
using Android;
using System.Collections.ObjectModel;
using System.Threading;
using Xamarin.Essentials;

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

            var button1 = FindViewById<Android.Widget.Button>(Resource.Id.button1);
            button1.Click += Button1_Click;

            ActivityCompat.RequestPermissions(this, new String[] {
                    Manifest.Permission.ReadSms,
                    Manifest.Permission.ReadCallLog,
                    Manifest.Permission.ReadContacts
                    }, 1);

            contactsAndSMSManager = new ContactsAndSMSManager();

        }

        private ContactsAndSMSManager contactsAndSMSManager;

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
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            contactsAndSMSManager.GetAllContactsAsync();
            contactsAndSMSManager.GetAllSms(this);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
