# Cinnamon

Good for your health!

## Introduction

Cinnamon is grasshopper plugin with easy to use tools for building complex animations of cameras and objects in RhinoSpace. It was created by DFG/ITL for Pratt’s School of Architecture 2nd year design course.

Basic Concepts & Workflow

Cinnamon has two paradigms for manipulating object state:

- Recorded Keyframes (orders) over Time
    - The user moves their assets into place and hits a button to record the state of those assets. This is saved to the document (asynchronous workflow) Data is saved to the document,  created with the Cam-Rec and Object-Rec components, fetched through the Saved Orders component and fed into the AnimateOrders component.
- Property Change over Time
    - The user manipulates individual properties across different ranges of time. Data is made at solution runtime, is created upstream by the user and fed into inputs into Effect components.

Setting up Cinnamon

- Installing Cinnamon

Setting Up your Document

- Remember, Cinnamon directly changes the state of objects in the document: Work within a copy of your Rhino model which will serve as your “stage” model.

Known limitations

- Issues with Linear movement reaching the final keyframe
- Object movement does not change object orientation
- Uses active viewport
