# Blog Post #05 - VR Project Idea & Initial Steps
### Author: Viktor Ivaylov Ivanov

## The Big Idea: Expanding Chemistry into Virtual Reality

The goal of this project is to further explore how immersive technologies can support learning in Chemistry, this time by shifting from Augmented Reality to Virtual Reality. While the core educational concept remains the same (understanding chemical elements and how they combine into molecules), the transition to VR enables a more immersive and spatially rich learning experience.

Instead of interacting with physical cards, the user is placed inside a virtual chemistry lab. Individual atoms can be grabbed, moved, and combined using natural hand-based interactions. When the correct combination of atoms is brought together, the system forms the corresponding molecule in real time. This approach allows users to explore molecular structures at human scale, something that is difficult to achieve using traditional learning methods or even AR.

By keeping the core idea consistent with the previous AR project, this iteration focuses on depth rather than reinvention, which allowed more time to be spent on VR-specific interaction design and user experience.

## Technical Stack

The project was developed using Unity, which was again chosen due to familiarity and its strong support for immersive technologies. For VR functionality, Unity’s XR Interaction Toolkit was used as the primary framework. It provides a standardized way to handle VR input, interaction, and locomotion across different devices.

A key design decision was the reuse of assets and logic from the AR project, including atom and molecule prefabs. By avoiding additional asset research and creation, development time could be invested into learning and implementing VR-specific concepts, such as spatial interaction, comfort-focused locomotion, and hand-based manipulation.

## First steps: Project Setup

With the core idea established, the next step was to initialize the VR project and set up a development environment suitable for immersive interaction.

### Reusing Assets from the AR Project

A key design decision at this stage was to reuse assets and logic from the previous AR project. This included:

- Atom & molecule prefabs and visual representations
- Reaction logic and data structures, which was only used for reference, as the code needs rewriting.

### Initial Scene Setup

After the assets were migrated, a basic VR scene was assembled. This included:

- An XR Origin with action-based input
- A simple lab environment to provide spatial context
- Temporary atom placement areas for early interaction testing

At this stage, the focus was not on visual polish, but on ensuring that core interactions such as grabbing, moving, and releasing atoms worked reliably in VR. 

