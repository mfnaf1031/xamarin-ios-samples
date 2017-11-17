﻿// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using CoreSpotlight;
using System.Collections.Generic;

namespace Todo11App
{
    public partial class DetailViewController 
    {
        // this gets called periodically after activity.BecomeCurrent() is called
        // //http://www.raywenderlich.com/84174/ios-8-handoff-tutorial
        public override void UpdateUserActivityState(NSUserActivity activity)
        {
            Console.WriteLine("UpdateUserActivityState for " + activity.Title);
            // update activity 
            if (Current.IsIndexable())
            {
                activity.AddUserInfoEntries(Current.IdToDictionary());
            }
            // persist partly-entered data for Handoff
            activity.AddUserInfoEntries(Current.NameToDictionary());
            activity.AddUserInfoEntries(Current.NotesToDictionary());

            base.UpdateUserActivityState(activity);
        }
        public override void RestoreUserActivityState(NSUserActivity activity)
        {
            base.RestoreUserActivityState(activity);
            Console.Write("RestoreUserActivityState ");
            if ((activity.ActivityType == ActivityTypes.Detail)
                || (activity.ActivityType == ActivityTypes.Add))
            {
                Console.WriteLine("NSUserActivity " + activity.ActivityType);
                if (activity.UserInfo == null || activity.UserInfo.Count == 0)
                {
                    // new todo 
                    Current = new TodoItem();
                }
                else
                {
                    // load existing todo
                    var id = activity.UserInfo.ObjectForKey(ActivityKeys.Id).ToString();
                    Current = AppDelegate.Current.TodoMgr.GetTodo(Convert.ToInt32(id));

                    if (Current == null) Current = new TodoItem();
                    // extract information from UserActivity to override local data -- "maybe" a good idea, maybe not...
                    Current.Name = activity.UserInfo.ObjectForKey(ActivityKeys.Name).ToString();
                    Current.Notes = activity.UserInfo.ObjectForKey(ActivityKeys.Notes).ToString();
                }
            }
            if (activity.ActivityType == CSSearchableItem.ActionType)
            {
                Console.WriteLine("CSSearchableItem.ActionType");
                var uid = activity.UserInfo[CoreSpotlight.CSSearchableItem.ActivityIdentifier];

                Current = AppDelegate.Current.TodoMgr.GetTodo(Convert.ToInt32(uid.Description));

                Console.WriteLine("eeeeeeee RestoreUserActivityState " + uid);
            }

            // CoreSpotlight index can get out-of-date, show 'empty' task if the requested id is invalid
            if (Current == null)
            {
                Current = new TodoItem { Name = "(not found)" };
            }
        }


        //[Export ("textFieldShouldReturn:")]
        //public bool ShouldReturn (UITextField textField)
        //{
        //  textField.ResignFirstResponder ();
        //  return true;
        //}

        [Export("textFieldDidEndEditing:")]
        public void EditingEnded(UITextField textField)
        {
            this.UserActivity.NeedsSave = true; // push partially entered data into UserInfo for Handoff
        }
    }
}
