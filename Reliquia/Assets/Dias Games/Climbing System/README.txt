RELEASE NOTES 

ALWAYS BACKUP YOUR PROJECT BEFORE ANY UPDATE

RELEASE NOTES - v3.1
• New inspector look.
• Climb for objects with roational speed.
• Auto-climb for ladders, vaults and lower climbs.
• Climb IK improved.
• New modifier manager: easy to edit.


RELEASE NOTES - v3.0 <br> <br>
• Major Upgrade for Climbing System with a lot of new features:
- New abilities: Wall Running, Wall Climb, Crawl.
- New math for climb jumps: all climb jumps now calculate a path to the target making character jump precisely to the target.
- Support for Mobile.
- Support for 2.5D.
- New way to change input for any ability.
- New animation.


VERSION 2.3

 - Fixed bug with ignore abilities losing data after add a new ability to the list.
 - Added option to drop climb to avoid drop ledge when moving fast.
- Changed crouched animations
- Internal update to work with Shooter System.
- Minor changes to find ledge more accurate.

VERSION 2.2

- NEW Window: set input manager and layer collisions by a new window.
- Fixed positioning problem in low fps. Now character fits position in any circumstances.

VERSION 2.01

Documentation updated
Small fixes on layers and input settings

VERSION 2.0

☺ NEW FEATURES ☺
  • Vault and Crouch abilities were added.
  • New Camera Controller system.
  • Object Pooling and Events.
  • Health Controller.
  • Ragdoll controller.
  • InControl integration.


- Climbing System is now part of Third Person System: 
- Climbing System contains everything that is in Third Person System.
- Now, you need to download only climbing animations from Mixamo. Locomotion, Jump, Roll, Crouch and more are available in the package.
- Follow the new tutorials showing how to get animations, setup your project, create new abilities, and more: https://www.youtube.com/playlist?list=PLiRDro5YzN40Ah0-_LQnvqVQnUrZfHHDq

/----------------------------/

• Changes made in version 2.0
  ○ Third Person Egnine class changed to Third Person System.
  ○ New inspector visual.
  ○ Invector integration was stopped. (But support continues for those who bought before version 2.0)




--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------

VERSION 1.4

This update has important changes. The core controller of Climbing System was changed to allow integration with third party assets and be more friendly for all costumers.

Important changes made in this version:
- Core controller modified from CharacterController to ThirdPersonEngine.
- Animator Controller now works only with 3 parameters.
- All animations transitions is now made by script. It allows better organization of the animator.
- It's not necessary to add animation events and curves in the animations. Everything works by script, including Hand and Foot IK.
- It's not necessary to have ledge limits on side to avoid climb. Casting were modified to auto detect situations that are not allowed to climb.

Abilities changes and features:
- Character now jump to any side from a ladder. Character can also drop from a ladder pressing drop button.
- Character now turn internal ledges.
- It's possible now to climb movable ledges and surfaces.
- Character will not climb up if there is no ground above.
- Jump in place animation were changed. See Animation List at Climbing system folder.

You can easily access the documentation in the link: https://www.dropbox.com/sh/j0aj0o4uuy7ao2u/AABiBKFFn4ZxRuWzrNpBG4vea?dl=0

ENJOY IT!
