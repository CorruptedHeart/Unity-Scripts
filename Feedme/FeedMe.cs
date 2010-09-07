// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: FeedMe.cs
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

public class FeedMe : MonoBehaviour
{
#region Variables
	// The URL to where the feed is located
	public string feedURL = "http://localhost/feed.xml";
	// A Struc of what every feed is made up of
	private struct feedThis
	{
    	public string Title;
    	public string Message;
		public string Date;
		public string Author;
	}
#region Feed Variables
	// Should the feed only update once per game play
	public bool updateOnce = false;
	// How long till the next update
	public int minutesTillUpdate = 60;
	// This is used to make sure that the text loaded doesn't fill more than the array
	private byte feedLength = 9;
	// This is the array of a struc of variables used to hold the feeds
	private feedThis[] theFeed = new feedThis[9];
	// When the feed is updated the time is set here plus the minutes to update
	private float timeSinceLastUpdate = 0.0f;
	// if the feed must be updated
	private bool updateFeed = true;
	// if the feed is updating
	private bool updating = false;
	// if there is no errors from the WWW connection
	private bool errorFree = true;
	// used to measure the number of elements within the array
	private byte arrayLength = 0;
#endregion
#region GUI
	// Skin for gui
	public GUISkin mySkin;
	// Keeps position of the scroll position
	private Vector2 scrollPosition;
	// The size and position of the feed window - The first two fields are the only ones required
	public Rect feedWinRect = new Rect(0,30,250,50);
	// the actual width of the feed window
	public int winWidth = 250;
	// the size of the feed button
	public int feedBtnWidth = 190;
	// the size of the reload button
	public int reloadBtnWidth = 30;
	// whether the feed window is being displayed or not
	private bool showFeed = false;
	// The icon to display on the feed button
	public Texture2D feedIcon;
	// The icon to display on the reload button
	public Texture2D reloadIcon;
	private GUIContent reloadBtn;
#endregion
#endregion
#region Standard Methods
	void Start()
	{
		// This adds a bit of error checking to see if there is a icon attached to the reloadIcon variable
		// and if there is none then it changes the button size to accomodate the extra width of the words reload
		// and fills the GUIContent with the words reload
		if(reloadIcon)
		{
			reloadBtn = new GUIContent(reloadIcon);
		}
		else
		{
			reloadBtn = new GUIContent("Reload");
			reloadBtnWidth = 50;
		}
		// Starts the getFeed Coroutine
		StartCoroutine(getFeed());
	}
	
	void Update()
	{
		// If it is set to update only once then it skips the next piece
		if(!updateOnce)
		{
			// Checks whether the feed needs to be updated
			if((Time.time >= timeSinceLastUpdate) && (!updateFeed))
			{
				// Resets the vars
				updateFeed = true;
				arrayLength = 0;
				// Starts the getFeed Coroutine
				StartCoroutine(getFeed());
			}
		}
	}
	
	void OnGUI()
	{
		// Creates the GUIContent for the button because it needs to be created for the error checking to work
		// correctly
		GUIContent feedBtn = new GUIContent("Feed: ");
		// creates an empty string for the feed button title
		string Feed = "";
		// If there was a skin set then set it else leave it as default
		if(mySkin != null)
		{
			GUI.skin = mySkin;
		}
		//To show that the feed is updating
		if(updating)
		{
			Feed =  "Is updating...";
		}
		// if the feed isn't updating show the latest feed title as button caption
		else
		{
			Feed = theFeed[0].Title;
		}
		// If there is an icon in the feedIcon var then it fills the GUIContent created above with the feed string and 
		// icon. Otherwise it creates a string representation of feed
		if(feedIcon != null)
		{	// Sets an icon and the feed string to a GUIContent
			feedBtn = new GUIContent("   " + Feed, feedIcon);
		}
		else
		{
			feedBtn = new GUIContent("Feed: " + Feed);
		}
		// Button for showing the field
		if(GUI.Button(new Rect(0,0,feedBtnWidth, 30),feedBtn))
		{
			// On click it simply switches the feed window on and off
			showFeed = !showFeed;
		}		
		// Shows the feed window based upon the value of showFeed
		if(showFeed)
		{
			feedWinRect = GUILayout.Window(1, feedWinRect, theFeedWindow, "Feeds", GUILayout.Width(winWidth));
		}
		// An update now button
		if(GUI.Button(new Rect(feedBtnWidth, 0, reloadBtnWidth, 30), reloadBtn))
		{
			// Resets the vars
			updateFeed = true;
			arrayLength = 0;
			// Starts the getFeed Coroutine
			StartCoroutine(getFeed());
		}
	}
#endregion
#region Custom Functions
	IEnumerator getFeed()
	{
		// if the feed must be updated run the function
		if(updateFeed)
		{
			// Sets updating to true
			updating = true;
			errorFree = true;
			// Gets the data from the url set in the inspector
			// This form is simply to make sure that WWW gets the latest version of the xml document
			WWWForm form = new WWWForm();
			form.AddField("number", 1221);
    		WWW feedPost = new WWW(feedURL, form);
			// Returns control to rest of program until it is finished downloading
    		yield return feedPost;
			// if there are no errors runs through the function
			if(feedPost.error == null)
			{
				// Creates a instance of TinyXmlReader to loop through the xml document
				// loops through the document and extracts data from each <Feed></Feed>
				// For my use, it extracts the title, date, author and message.
				// it stores this in an array of struc type declared above
				TinyXmlReader reader = new TinyXmlReader(feedPost.data);
				while(reader.Read("Feeds") && (arrayLength < feedLength))
				{
					while(reader.Read("Feed"))
					{
						if(reader.tagName == "Title" && reader.isOpeningTag)
						{
							theFeed[arrayLength].Title = reader.content;
						}
						if(reader.tagName == "Date" && reader.isOpeningTag)
						{
							theFeed[arrayLength].Date = reader.content;
						}
						if(reader.tagName == "Author" && reader.isOpeningTag)
						{
							theFeed[arrayLength].Author = reader.content;
						}
						if(reader.tagName == "Message" && reader.isOpeningTag)
						{
							theFeed[arrayLength].Message = reader.content;
						}
					}
					// Increments the counter variable for the array
					arrayLength++;
				}
			}
			// if there was an error
			else
			{
				// sets all the error messages
				errorFree = false;
				theFeed[0].Title = "Error connecting to Feed...";
				theFeed[0].Message = feedPost.error;
				// Logs an error
				Debug.LogError(feedPost.error);
			}
			// sets the values of the variables to there off state
			updateFeed = false;
			updating = false;
			// sets the last update time
			timeSinceLastUpdate = Time.time + (60 * minutesTillUpdate);
			// Destroys the feedPost...
			feedPost.Dispose();
		}
	}
#endregion
#region GUI Functions
	// The feed window GUI
	void theFeedWindow(int WindowID)
	{
		// Creates a empty string to hold all the feeds
		string Feeds = "";
		// Starts a scroll view
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(winWidth), GUILayout.Height(400));
		// if there are no errors, shows the feed as is
		if(errorFree)
		{
			// loops through the array adding the feed data to the feed string
			for(int x=0;x<=arrayLength-1;x++)
			{
				Feeds = Feeds + theFeed[x].Title + " - " + theFeed[x].Date + "\n"
							+ "Author: " + theFeed[x].Author + "\n" + theFeed[x].Message + "\n\n";
			}
		}
		// if there is an error sets the feed to just 0 and not a loop
		else
		{
			Feeds = theFeed[0].Title + "\n" + theFeed[0].Message;
		}
		// Creates a label from the feed string
		GUILayout.Label(Feeds);
		// ends the view
		GUILayout.EndScrollView();
	}
#endregion
}