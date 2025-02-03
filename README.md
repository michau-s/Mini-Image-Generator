# MiniImageGenerator

## Description

MiniImageGenerator was developed as part of the Programming 3 - Advanced course during the winter semester of 2024/2025 academic year.

It aims to provide a compact interface for generating and/or processing images in a unique way.

## Running the program
Make sure to change the architecture to `x64`, then compile and run the project. You will be prompted to type a chain of commands, as illustrated, you may also type `Help` for help.

<p align="center">
  <img src="./Docs/Images/Help.png" 
       alt="Help console output" 
       style="width: 80%;"/>
</p>

Using the software involves typing the command chain from the list of available commands, separated by the `|` character, in the following format:

```
generating_command arg1 arg2 | processing_command1 | processing_command2 arg1 arg2
```

For example, using the following command chain generates, in parallel, 10 images similar to the one below:

```
Generate 10 1024 1024 | Blur 4 4 | RandomCircles 10 50 | ColorCorrection 0,1 0,1 0,1 | GammaCorrection 0,9 | Output test
```

<p align="center">
  <img src="./Docs/Images/sample.jpeg" 
       alt="Sample Image" 
       style="width: 80%;"/>
</p>

One can also process images from the disk by starting the chain with `Input <filename>`. It is important to note that without the `Output <filename>` no images will be written to the disk. The default path is the one the executable is in.

After the processing starts, a progress bar appears in the console, individual processing commands are separated by the `|` character.

<p align="center">
  <img src="./Docs/Images/ProgressBar.png" 
       alt="Sample Image" 
       style="width: 80%;"/>
</p>

It is possible to cancel the execution of the program by pressing `x`, in this case, nothing gets written to the disk and the program safely terminates.

## Implementation
Five primary classes have been implemented:
- **ImageGenerator** - used for all "generate" methods.
- **ImageProcessor** - used for all methods processing methods.
- **InputParser** - used for checking if the input provided by the user is in the correct format.
- **ProgressReporter** - used for displaying and handling the progress bar through an event, each command periodically reports it's progress to the reporter.
- **MiscellaneousCommands** - For utility commands.

You are encouraged to take a look at the XML documentation for supplementary information about the implementation. Additionally, the original task description has been included below for convenience.

# Programming 3 - Advanced
## Interoperability - Project 2
*Original assignment description*

Your task is to create an interactive procedural image generator using a native library written in C++. Let's start with an...
### Example:

The user can enter a *generating command*:
- Generate `n` `width` `height`

The program creates `n` images whose size is `width`x`height` pixels and calls an external library to fill them with random patters.

The result of the command can then be piped into *processing commands* such as
- Blur `width` `height` 
- Output `filename_prefix`

that modify it or output the result into a file:
```
Generate 10 1024 1024 | Blur 4 4 | Output Image
``` 
The following chain of commands generates 10 1024x1024 images, blurs them with radius 4 vertically and horizontally and saves them as "Image1.jpeg", "Image2.jpeg", ... , "Image10.jpeg"

### Command chains
The user enters a *generating command* and its arguments, followed by 0 or more *processing commands* and their respective arguments. Commands are separated by the `|` (pipe) character.
```
generating_command arg1 arg2 | processing_command1 | processing_command2 arg1 arg2
```

#### Error checking
Detect syntax errors. It's sufficient to determine one has occurred. In such case don't start the execution of the entered command chain. 

### Progress reporting
Every library function accepts a progress-reporting callback that is called every 1% of the work:

C++
```cpp
bool TryReportCallback(float)
```
- The argument passed to the callback is the fraction of the work of the current function.
- If the callback returns `false`, the library function returns early.

Show execution progress of each image in the chain separately. Separate the progress of each command in the chain visually (segments of roughly equal size is sufficient).

Example output:
```
[######|######|###---|------|------] 50% 
[######|######|##----|------|------] 47% 
[######|######|###---|------|------] 50% 
[######|######|######|------|------] 60%
```

### Abort
When a command chain is being processed, the user can press `x`. All chains should then safely stop. Remember to free resources!

Tip: The progress-reporting callback returns a boolean. In order to stop, return false. 

### Commands
Implement at least 3 *generating commands* using the `GenerateImage_Custom` library function and 3 *processing commands* using the `ProcessPixels_Custom` library function. Get creative!


---
The following should be available in your project:

*Special commands*:
- Help
    - List available commands together with argument descriptions. Does nothing if called in a command chain. List the functions you came up with at the start.

*Generating commands*:
- Input `filename`
    - Load an image from the disk. (to be implemented)
- Generate `n` `width` `height`
    - Create `n` images whose size is `width`x`height` pixels (to be implemented) and fill them with random patterns (implemented in the library).
- *Your 3 commands* (to be implemented using `GenerateImage_Custom`)

*Processing commands*:
- Output `filename_prefix` 
    - Save images to the disk (to be implemented).
- Blur `w` `h`
    - Apply a `w`x`h` blur (implemented in the library).
- RandomCircles `n` `r`
    - Add `n` circles of radius `r` placed randomly on the images (to be implemented using `DrawCircles`).
- Room `x1` `y1` `x2` `y2`
    - Draw a filled rectangle with the given coordinates. The coordinates range from 0 to 1 (to be implemented using `ProcessPixels_Custom`).
- ColorCorrection `red` `green` `blue`
    - Apply color correction (implemented in the library).
- GammaCorrection `gamma`
    - Apply Gamma correction (implemented in the library).
- *Your 3 commands* (to be implemented using `ProcessPixels_Custom`)