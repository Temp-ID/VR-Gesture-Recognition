# VR Gesture Recognizer - Code Sample

This repository contains code samples for Gesture Recognition as it was written
for my VR game, Mr. Love Potion. This includes the core components needed to
import gesture recognition directly into another VR project, and is completely
agnostic to any underlying VR framework or any specific device 
(SteamVR, Oculus, etc).

This was written using C# and Unity 2021.3.6f1.

# What it is

https://github.com/Temp-ID/VR-Gesture-Recognition/assets/15268274/b2ccac8b-0343-42eb-9a93-d0228935b382

Gesture recognition for this project means understanding how a player is
currently posed. For example, a gesture to ask a question may recognize having
either the left or right hand straight up.

You can easily extend this idea to capture a sequence of separate poses to 
recognize actions:
- Waving a hand
- Sprinting
- Dancing!

https://github.com/Temp-ID/VR-Gesture-Recognition/assets/15268274/26dd6fca-b6fa-464e-994d-cdac7b771447

Reasons I've used gesture recognition:
- To assert dominance over black bears
- Ask questions to NPCs
- Initiate handshakes
- Point at objects
- Have dance-offs, awarding points for correct gestures at correct times

## How it Works

Gesture recognition is done by performing polar-coordinate calculations between 
the user's hand and estimated shoulder position to determine the direction 
the user is pointing their hand. We have to estimate the shoulder position for 
this calculation as it is the most intuitive and practical way to define a gesture.
In real-life, if we want to check if a user's hand is pointed straight up, we
identify that gesture based on where the hand is relative to the shoulder!

Gestures are considered fulfilled if (for both hands):
* The hand in question is rotated appropriately
   * A hand straight out, facing up may mean you want money
   * A hand straight out, facing sideways may mean you want a handshake
* The hand is point away from estimated shoulder joints correctly
   * Hands pointed outwards from the body may mean you want a hug
   * Hands pointed straight inwards from the shoulder may mean you're a zombie

The polar-coordinate calculations are performed in [HandGesture.cs](https://github.com/Temp-ID/VR-Gesture-Recognition/blob/main/Code/Gestures/HandGesture.cs)

# What's Included

All components for basic gesture recognition are included. In addition, this
sample provides in-editor manipulation and preview of gesture definitions, so
you can easily edit and preview gestures with immediate visual feedback to
ensure your changes accurately reflect the gesture you want to perform.

https://github.com/Temp-ID/VR-Gesture-Recognition/assets/15268274/9f8e5b5a-92e3-435c-91f1-26f6ef259182

# Code Structure

The code structure presented here can be thought as a module that can be 
inserted into any VR project.
