// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: WriteFeed.cs
//  
// Author: Garth "Corrupted Heart" de Wet <mydeathofme[at]gmail[dot]com>
// Website: www.CorruptedHeart.co.cc
// 
// Copyright (c) 2010 Garth "Corrupted Heart" de Wet
//  
// Permission is hereby granted, free of charge (a donation is welcome at my website), to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class WriteFeed : MonoBehaviour
{
#region Variables
#region URLS
	// The URL to where the feed is located
	public string NewFeedURL = "http://localhost/create.php";
#endregion
	// A Struc of what every feed is made up of
	private struct FeedThis
	{
    	public string Title;
    	public string Message;
		public string Author;
	}
#region Feed Variables
	// This is the array of a struc of variables used to hold the feeds
	private FeedThis NewFeed = new FeedThis();
	// if the feed must be updated
	private bool Updated = false;
	// if the feed is updating
	private bool Updating = false;
	// if there is no errors from the WWW connection
	private bool ErrorFree = true;
	private string FeedBack = "";
#endregion
	
#region GUI
	// Skin for gui
	public GUISkin MySkin;
	// Keeps position of the scroll position
	private Vector2 ScrollPosition;
	// The size and position of the feed window - The first two fields are the only ones required
	public Rect FeedWinRect = new Rect(270,30,250,400);
	// The size and position of the feed window - The first two fields are the only ones required
	public Rect FeedBtnWidth = new Rect(220,0,35,30);
	// The width of the window
	public int FeedWidth = 250;
	// The height of the window
	public int FeedHeight = 400;
	// whether the feed window is being displayed or not
	private bool ShowFeed = false;
	// The icon to display on the feed button
	public Texture2D PostFeedIcon;
#endregion
#endregion
	
#region Standard Methods
	void Start()
	{
		// Setting the feed to empty in order to allow the input to work
		NewFeed.Author = "";
		NewFeed.Message = "";
		NewFeed.Title = "";
	}
	
	void OnGUI()
	{
		// If there was a skin set then set it else leave it as default
		if(MySkin != null)
		{
			GUI.skin = MySkin;
		}
		// Button for showing the field
		if(GUI.Button(FeedBtnWidth,PostFeedIcon))
		{
			// On click it simply switches the feed window on and off
			ShowFeed = !ShowFeed;
		}
		// Shows the feed window based upon the value of showFeed
		if(ShowFeed)
		{
			// creates the window
			FeedWinRect = GUILayout.Window(2, FeedWinRect, theFeedWindow, "New Feed", GUILayout.Width(FeedWidth));
		}
	}
#endregion
#region Custom Functions
	IEnumerator PostFeed()
	{
		// Sets updating to true
		Updating = true;
		
		// Creates a WWWForm in order to fill in the details that will be submitted to the php script
		WWWForm theFeed = new WWWForm();
		// Adds the Title
		theFeed.AddField("title", NewFeed.Title);
		// Adds the message
		theFeed.AddField("message", NewFeed.Message);
		// Adds the name
		theFeed.AddField("name", NewFeed.Author);
		
		// Sends the form and gets the reply from the server
   		WWW feedPost = new WWW(NewFeedURL, theFeed);
		// Returns control to rest of program until it is finished downloading
   		yield return feedPost;
		// if there are no errors runs through the function
		if(feedPost.error == null)
		{
			FeedBack = feedPost.data;
			Updated = true;
		}
		// if there was an error
		else
		{
			// sets all the error messages
			ErrorFree = false;
			// Logs an error
			Debug.LogError(feedPost.error);
			FeedBack = feedPost.error;
		}
		// sets the values of the variables to there off state
		Updating = false;
		// Destroys the feedPost...
		feedPost.Dispose();
	}
#endregion
#region GUI Functions
	// The feed window GUI
	void theFeedWindow(int WindowID)
	{
		// Starts a scroll view
		ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(FeedWidth), GUILayout.Height(FeedHeight));
		// if it is not error free create a label with the error code
		if(!ErrorFree)
		{
			GUILayout.Label("Error: " + FeedBack);
		}
		// if it was updated then clear the inputs and put a label with the answer from the server
		if(Updated)
		{
			NewFeed.Author = "";
			NewFeed.Message = "";
			NewFeed.Title = "";
			Updated = false;
		}
		// if it is currently updating then display that in a label or else display the input area
		if(Updating)
		{
			GUILayout.Label("Feed is being updated...");
		}
		else
		{
			GUILayout.Label(FeedBack);
			GUILayout.Label("Enter Your Name:");
			NewFeed.Author = GUILayout.TextField(NewFeed.Author);
			GUILayout.Label("Enter The Title:");
			NewFeed.Title = GUILayout.TextField(NewFeed.Title);
			GUILayout.Label("Enter Your Message:");
			NewFeed.Message = GUILayout.TextArea(NewFeed.Message);
			
			if(GUILayout.Button("Submit"))
			{
				Updated = false;
				StartCoroutine(PostFeed());
			}
			
			if(GUILayout.Button("Reset"))
			{
				NewFeed.Author = "";
				NewFeed.Message = "";
				NewFeed.Title = "";
			}
		}
		// ends the view
		GUILayout.EndScrollView();
	}
#endregion
}