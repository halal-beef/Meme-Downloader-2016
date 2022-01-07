# Meme Downloader 2016

That name is just joking :)

Dependencies (handled by the program):

  - ffmpeg
  - youtube-dlp

This app downloads can download stuff from a subreddit of your choice and throws what it gets into a folder named `Downloaded Contents`, which inside has folders with the names of the subreddits of your choice.

This app runs 32 threads by default, and is (sometimes) VERY resource hungry on memory, some processes might stay as zombies, that means, they don't die, examples are ffmpeg, that can prevent you from deleting the program 

**This app has some dependencies on Windows**: 

  - taskkill

## `config.json` template

This are the default values of `config.json` after being created:

```
{
	"TargetSubReddit0": "shitposting",
	"TargetSubReddit1": "dankmemes",
	"TargetSubReddit2": "memes",
	"TargetSubReddit3": "crappyoffbrands",
	"TargetSubReddit4": "nocontextpics",
	"TargetSubReddit5": "funny",
	"MaxRepeatTimes": 10000,
	"SimultaneousDownload": true,
	"ThreadCount": 32
}
```
**TargetSubreddit(0, 1, 2, 3, 4, 6)**: This are the subreddits from where you want to get posts from <br>
**MaxTimesRepeated**: This is the max amount of times the bot has to get a "It already exists" to change and download from another subreddit<br>
**SimultaneousDownload**: This enables the program to download from two subreddits simultaneously<br>
**ThreadCount**: This is how many bots will run simultaneously, going over 32 is normally overkill

------------------

### **NOTES**:

 - ~~After restarting the program after installing the dependencies, it will fail to run, please, close it forcefully and open it back again to fix this issue~~ (Fixed)

------------------

Credits:

  - Original Idea: [SmallPP420](https://github.com/SmallPP420)

------------------

Thanks to: 

  - TeNaihi for helping me test it :D

------------------


## NOTES FOR FORKERS

If you fork this repo you will need to add a repository secret named **TEST** with your github token as your value if u want to use the github ci script to build
