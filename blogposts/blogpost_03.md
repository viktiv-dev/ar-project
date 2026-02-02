# Blog Post #3 - Creating Assets

### Author: Viktor Ivaylov Ivanov
## Project Setup 
The first step of the project was initializing it in Unity. The project was created using Unity’s AR Core Template, which provides a preconfigured setup for camera access, tracking, and AR session management. After removing the default template content, the focus shifted to building the core assets and systems required for the project.

## Creating Assets

### Images
The first assets to be imported were the element card images. After importing the images into Unity, an XR Reference Image Library was created with each card added as a tracked image.

This reference library allows AR Foundation to recognize specific images in the real world and anchor virtual content to them. Each card acts as a trigger point for spawning its corresponding atom, making the physical cards an essential part of the interaction loop.

### Atoms & Molecules
#### Atoms:
Atoms were generated using a custom AtomGenerator script. This script handles both the visual construction of atoms and their continuous visual updates. This ensures consistency, both visual and structure wise.

Each atom consists of:

- A nucleus placed at the center
- One or more electrons orbiting the nucleus
- Randomized orbital orientations to avoid uniform, artificial-looking movement

The electrons rotate around pivot points rather than being physically simulated, which keeps performance stable and avoids unnecessary physics complexity. 

#### Molecules:
Molecules were created manually rather than being procedurally generated from atoms. The initial idea was to dynamically generate molecules based on the atoms detected in the scene, but due to time constraints and the complext structure and shape of molecules, a simpler and more controlled approach was chosen.

Each molecule is constructed using primitive spheres to represent atoms, with bonds connecting them using the BondBuilder script. This script allows two atoms to be selected and connected by a cylindrical bond that automatically positions, rotates, and scales itself between them.

While this approach sacrifices some procedural flexibility, it provides:

- Full control over molecular structure
- Visual clarity
- Faster iteration during development

To enhance presentation, completed molecules were given subtle animation using a MoleculeSpinner script, which added a slow rotation and gentle bobbing to make them feel alive and engaging in AR space.


 
