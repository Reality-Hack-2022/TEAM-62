# MetaHack
> this project was created for Hacking The Hack, the remote Hackathon of MIT Reality Hack

A way to connect Unity and Web app through WebRTC

## Inspiration
During the Hacking The Hack remote hackathon, when I was seeing everyone pitching their ideas, I was thinking that it looks like separate apps, interesting, but not so meta...
![miro](https://user-images.githubusercontent.com/3470988/160245724-cba225c6-38ed-4f22-a94e-2f195b677f03.PNG)

## What it does
Enable interoperability between projects through a unity and web compatible library based on WebRTC.

## How we built it
- using webrtc unity preview package
- newtonsoft json
- glitch as backend for signaling server + front for demo

## Challenges we ran into
- the project isn't appealing as it's an in-between project and not an app that can run alone
- webrtc unity preview package doesn't act exactly as webrtc on the web... I've lost too much time finding where my assumptions were false about how it should work...
- in remote, it's not easy to discuss with on-site teams, as a project that should be used by everyone, not being able to easily communicate is a big handicap
- until the lib is working, I can't works with others to integrate it because it's useless... I was thinking to be able to finish it on 25, but too much problems encountered, so I don't know if I'll be able to integrate the library into projects before the end

## Accomplishments
- at least now, an alpha version is working!

## What have been learned
- would have been easier to do a little XR apps for the hackathon... but I expect that my lib will at least be integrated in two projects before the end!

## What's next for MetaHack
- could be really interesting to properly "finish" it and publish it as a unity package and web library
- a lot of things should be done before interoperability will appears

## Built With
- c#
- javascript
- unity
- web
- webrtc
- webxr

## How to setup MetaHack in Unity
- copy the MetaHack folder in your unity project
- [install WebRTC package](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/install.html) (package manager, add package from git url "com.unity.webrtc@2.4.0-exp.6")
- install Newtonsoft Json (package manager, add package from git url "com.unity.nuget.newtonsoft-json@3.0")

For Windows, you need to target architecture x86_64

For Unity Android platform, you need to configure player settings below.
- Scripting backend - IL2CPP
- Target Architectures - ARM64 (Do disable ARMv7)

For others platforms, check https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/install.html

- As a note, it doesn't works on Unity to WebXR export (probably available after the hack)
- For now, check the repo as an example in unity
- After the hack, it will available as a unity package

## How to setup MetaHack in Web(XR)
- add `<script type="module" src="https://metahack.glitch.me/metahack.js"></script>` in your html `<head>`
- use `metahack` in `window.addEventListener("load", () => {});` callback (after the hack, it will also be usable outside of load)
- you can check https://metahack.glitch.me/ or https://metahack-demo.glitch.me/ as an example
