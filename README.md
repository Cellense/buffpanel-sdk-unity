# Tributit Unity SDK

This repository contains an implementation with a simple interface designed to easily communicate with the REST API of the Tribut.it service in your Unity Game.

Also included are files for the creation of test project for debugging purposes.

## Setup

To use the Unity SDK in your game all you need to do is copy the contents of the `/Assets/Scripts/TributitSDK/` folder in this repository into your game project.

## Start tracking

The SDK currently enables you to notify the Tribut.it server that a **player** with the supplied `player_token` (a unique identifier of the player, for instance her Steam Id) has triggered an **event** (for instance the installation of your game) that is being tracked by a campaign with the supplied `campaign_name` (The name of the campaign, that you have created on the Tribut.it server, please refer to <http://trbtit-cellense.rhcloud.com/> for more information).

Include this line of code in the place in your project's code, where you wish to track the **event**

```
Tributit.Tributit.Track(campaign_name, player_token);
```

Where `campaign_name` and `player_token` are of type `string`.

## How it works?

When called, the SDK attempts to notify one of our servers sequencially in a predetermined order. If an attempt would fail the next server is used until an attempt has been made on all servers.

## Callbacks

You can add an event handler by extending the supplied `Tributit.Callback` interface and implementing both its methods.

If a request attempt succeeds, the method `void success(WWW www)` is called and if it fails the method `void error(WWW www)` is called instead. In both methods the `WWW www` object contains all relevant information regarding the request.

To use your extended version of the `Tributit.Callback` interface (let's say you called it `MyCallback`), you need to include this line of code in the place in your project's code, where you wish to track the **event**

```
Tributit.Tributit.Track(campaign_name, player_token, new MyCallback());
```
