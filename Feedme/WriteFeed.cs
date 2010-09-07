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
	// The URL to where the feed is located
	public string feedURL = "http://localhost/create.php";
	// A Struc of what every feed is made up of
	private struct FeedThis
	{
    	public string Title;
    	public string Message;
		public string Author;
	}
#region Feed Variables
	// This is the array of a struc of variables used to hold the feeds
	private FeedThis newFeed = new FeedThis();
	// if the feed must be updated
	private bool updated = false;
	// if the feed is updating
	private bool updating = false;
	// if there is no errors from the WWW connection
	private bool errorFree = true;
	private string feedBack = "";
#endregion
#region GUI
	// Skin for gui
	public GUISkin mySkin;
	// Keeps position of the scroll position
	private Vector2 scrollPosition;
	// The size and position of the feed window - The first two fields are the only ones required
	public Rect feedWinRect = new Rect(270,30,250,400);
	// The size and position of the feed window - The first two fields are the only ones required
	public Rect feedBtnWidth = new Rect(220,0,35,30);
	// The width of the window
	public int feedWidth = 250;
	// The height of the window
	public int feedHeight = 400;
	// whether the feed window is being displayed or not
	private bool showFeed = false;
	// The icon to display on the feed button
	public Texture2D postFeedIcon;
#endregion
#endregion
	
#region Standard Methods
	void Start()
	{
		// Setting the feed to empty in order to allow the input to work
		newFeed.Author = "";
		newFeed.Message = "";
		newFeed.Title = "";
	}
	
	void OnGUI()
	{
		// If there was a skin set then set it else leave it as default
		if(mySkin != null)
		{
			GUI.skin = mySkin;
		}
		// Button for showing the field
		if(GUI.Button(feedBtnWidth,postFeedIcon))
		{
			// On click it simply switches the feed window on and off
			showFeed = !showFeed;
		}
		// Shows the feed window based upon the value of showFeed
		if(showFeed)
		{
			// creates the window
			feedWinRect = GUILayout.Window(2, feedWinRect, theFeedWindow, "New Feed", GUILayout.Width(feedWidth));
		}
	}
#endregion
#region Custom Functions
	IEnumerator PostFeed()
	{
		// Sets updating to true
		updating = true;
		// Creates a WWWForm in order to fill in the details that will be submitted to the php script
		WWWForm theFeed = new WWWForm();
		// Adds the Title
		theFeed.AddField("title", newFeed.Title);
		// Adds the message
		theFeed.AddField("message", newFeed.Message);
		// Adds the name
		theFeed.AddField("name", newFeed.Author);
		// Sends the form and gets the reply from the server
   		WWW feedPost = new WWW(feedURL, theFeed);
		// Returns control to rest of program until it is finished downloading
   		yield return feedPost;
		// if there are no errors runs through the function
		if(feedPost.error == null)
		{
			feedBack = feedPost.data;
			updated = true;
		}
		// if there was an error
		else
		{
			// sets all the error messages
			errorFree = false;
			// Logs an error
			Debug.LogError(feedPost.error);
			feedBack = feedPost.error;
		}
		// sets the values of the variables to there off state
		updating = false;
		// Destroys the feedPost...
		feedPost.Dispose();
	}
#endregion
#region GUI Functions
	// The feed window GUI
	void theFeedWindow(int WindowID)
	{
		// Starts a scroll view
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(feedWidth), GUILayout.Height(feedHeight));
		// if it is not error free create a label with the error code
		if(!errorFree)
		{
			GUILayout.Label("Error: " + feedBack);
		}
		// if it was updated then clear the inputs and put a label with the answer from the server
		if(updated)
		{
			newFeed.Author = "";
			newFeed.Message = "";
			newFeed.Title = "";
			updated = false;
		}
		// if it is currently updating then display that in a label or else display the input area
		if(updating)
		{
			GUILayout.Label("Feed is being updated...");
		}
		else
		{
			GUILayout.Label(feedBack);
			GUILayout.Label("Enter Your Name:");
			newFeed.Author = GUILayout.TextField(newFeed.Author);
			GUILayout.Label("Enter The Title:");
			newFeed.Title = GUILayout.TextField(newFeed.Title);
			GUILayout.Label("Enter Your Message:");
			newFeed.Message = GUILayout.TextArea(newFeed.Message);
			if(GUILayout.Button("Submit"))
			{
				updated = false;
				StartCoroutine(PostFeed());
			}
			
			if(GUILayout.Button("Reset"))
			{
				newFeed.Author = "";
				newFeed.Message = "";
				newFeed.Title = "";
			}
		}
		// ends the view
		GUILayout.EndScrollView();
	}
#endregion
}