# LiveBot
Simple C# Facebook Live Stream Creator and Comment Viewer (Twitch alike)

# Usage
1. Build it From Visual Studio 2015. Build As Debug / Release
2. Make config.json in same folder as Application folder.
```
{
  "token": "<Your FB User App Token Here>",
  "title": "<Your Stream Title>",
  "description": "<Your Stream Description",
  "save_vod": true,
  "status": "LIVE_NOW"
}
```
3. Start Executable file. Access Comment section at http://127.0.0.1/getComments/ (Psh, use OBS Browser to attach them to Your Stream, add Refresh Interval around 5 Seconds or More)


# Sources
WebServer Class : https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server
Package Used : RESTSharp, Newtonsoft.Json
