# Blog Post #6 - Development: Locomotion, Interaction & Core Systems
### Author: Viktor Ivaylov Ivanov
With the project initialized and assets migrated, development shifted toward implementing the core VR interactions that define the experience. The primary focus was on locomotion, object interaction, and the logic that connects user actions to chemical outcomes.

## Teleportation
Teleportation-based locomotion was chosen as the primary movement method. This approach avoids continuous camera motion, making it more comfortable and reducing the risk of motion sickness. It also fits well with the structured layout of the lab, allowing users to quickly move between interaction zones such as atom sources, processing areas, and displays while also exploring other areas. Using the XR Interaction Toolkit’s built-in teleportation system made implementation straightforward and reliable.

## Atom Interaction

Atoms are spawned at predefined locations and can be grabbed using hand-based interactions. While an atom is held, its movement is controlled by the XR interaction system to ensure stability and precision. When the atom is released, physics is re-enabled so it behaves naturally in the environment. To maintain a smooth interaction flow, atoms are automatically respawned after being taken, ensuring that the user always has elements available for experimentation.

## Lab Logic & Feedback

To manage the overall interaction flow, a central lab management system was introduced. This system acts as the coordinator between user input, visual feedback and molecule creation.

As atoms are added to the processsing area, the lab interface updates in real time, visually representing which elements have been inserted. Then, when the user initiates processing, the system temporarily locks further input and displays a short analysis phase. This delay, combined with visual indicators such as progress bars and status text, gives weight to the action and reinforces the idea that a chemical reaction is taking place.

If a valid combination of atoms is detected, the corresponding molecule is generated and presented along with contextual information such as its formula, polarity, physical state, and common uses.

## Data-Driven Molecule Definitions

Molecules are defined using data-driven recipes rather than hard-coded logic. Each recipe specifies:
- The required atoms and their quantities
- The resulting molecule prefab
- Descriptive scientific information