﻿using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using Android.Graphics;
using Fill_Up_App.Code.Exceptions;
using Fill_Up_App.Code.Extensions;
using System.Text.RegularExpressions;
using FillUpApp.Standart;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Fill_Up_App.Code.Data;

namespace Fill_Up_App.Code.Activities
{
    [Activity(Label = "Fill Up!")]
    public class TakePictureActivity : Android.App.Activity
    {
        ImageView imageView;
        int result = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TakePictureLayout);

            Button savebutton = FindViewById<Button>(Resource.Id.savebutton);
            savebutton.Click += new EventHandler(this.savebutton_Click);

            Button gobackbutton = FindViewById<Button>(Resource.Id.goBackButton);
            gobackbutton.Click += new EventHandler(this.gobackbutton_Click);

            Button openPicbutton = FindViewById<Button>(Resource.Id.openPictureButton);
            openPicbutton.Click += new EventHandler(this.openPicbutton_Click);

            Button buttonCam = FindViewById<Button>(Resource.Id.takePictureButton);
            imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            imageView.SetImageResource(Resource.Drawable.beerIconPng);
            buttonCam.Click += ButtonCam_Click;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)
            {
                if (resultCode == Result.Ok)
                {
                    Bitmap bitmap = (Bitmap)data.Extras.Get("data");
                    imageView.SetImageBitmap(bitmap);
                    PictureAnalysis pa = new PictureAnalysis();
                    result = pa.GetBeerPercentage(bitmap);
                }
                else return;
            }
            else if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    imageView.SetImageURI(data.Data);
                    imageView.BuildDrawingCache(true);
                    Bitmap bmap = imageView.GetDrawingCache(true);
                    imageView.SetImageBitmap(bmap);
                    Bitmap b = Bitmap.CreateBitmap(imageView.GetDrawingCache(true));
                    PictureAnalysis pa = new PictureAnalysis();
                    result = pa.GetBeerPercentage(b);
                }
                else return;
            }
        }

        private void ButtonCam_Click(object sender, EventArgs a)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
        }

        void savebutton_Click(Object sender, EventArgs e)
        {
            try
            {
                if (((EditText)FindViewById(Resource.Id.barName)).Text == string.Empty)
                {
                    throw new BarNameEmptyException("Įveskite baro pavadinimą");
                }

                if (!Regex.IsMatch(((EditText)FindViewById(Resource.Id.barName)).Text, @"^[\p{L}0-9\s]*$"))
                {
                    throw new RegexException("Pavadinime gali būti tik raidės, skaičiai ir tarpai!");
                }

                DatabaseProcessing.InsertToTable(((EditText)FindViewById(Resource.Id.barName)).Text, (int)((RatingBar)FindViewById(Resource.Id.ratingOfBar)).Rating);

                Intent intent = new Intent(this, typeof(ResultsActivity));
                Bundle bundle = new Bundle();
                bundle.PutString("name", ((EditText)FindViewById(Resource.Id.barName)).Text);
                bundle.PutDouble("mug", (((EditText)FindViewById(Resource.Id.orderedMug)).Text).StringToDouble());
                bundle.PutInt("rating", (int)((RatingBar)FindViewById(Resource.Id.ratingOfBar)).Rating);
                bundle.PutInt("result", result);
                intent.PutExtras(bundle);
                StartActivity(intent);

                FillUpWeb.FillUpWebService client = new FillUpWeb.FillUpWebService();
                bool value = client.AddBarReview(((EditText)FindViewById(Resource.Id.barName)).Text, 
                    (int)((RatingBar)FindViewById(Resource.Id.ratingOfBar)).Rating);
            }
            catch (BarNameEmptyException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (RegexException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, "Įvyko klaida!", ToastLength.Long).Show();
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            } 
        }

        void gobackbutton_Click(Object sender, EventArgs e)
        {
            Finish();
        }

        void openPicbutton_Click(Object sender, EventArgs e)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 1);
        }
    }
}