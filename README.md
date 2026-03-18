# Graveyard Rush
A Fast-Paced 3D FPS Survival Experience Developed for CSE 4410: Game Programming (Spring '26) @ California State University, San Bernardino

# Overview
Graveyard Rush is a stylized first-person shooter where you are trapped in a haunted cemetery. Armed only with a high-powered shotgun, you must survive increasing waves of zombies, manage your movement to avoid being cornered, and achieve the highest score possible before the horde overwhelms you.

#Key Technical Features
Dynamic AI Pathfinding: Zombies utilize Unity’s NavMesh system to hunt the player through complex graveyard geometry.

Advanced Physics Combat: Implemented BoxCastAll for realistic shotgun spreads and custom Vector3-based knockback to handle impact without physics jitter.

Global Game Architecture: Features a ScriptableObject-based Settings System and a Singleton Pattern for seamless scene transitions and audio management.

Atmospheric 3D Sound: Fully integrated Spatial Audio for enemies and a centralized Audio Mixer for dynamic volume control.

# Credits & Acknowledgments
Lead Developer: Dylan Cusson

Art Assets: 3D models and environment kits provided by Kenney.nl

Tools Used: Unity 6, Universal Render Pipeline (URP), and C#.

# How to Play
Move: WASD

Shoot: Left Click

Goal: Blast the zombies to push them back and survive as long as possible!
