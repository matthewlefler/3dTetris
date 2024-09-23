# 3D Tetris
A 3 dimensional Tetris-like game that is made using the MonoGame library in C#, meant to run on Devcade (a arcade-like cabinet made by CSHers) 

### Building with build scripts

For Linux / Mac

```sh
. ./publish.bash [Path to banner] [Path to icon]
```

For Windows

```sh
./publish.ps1 [Path to banner] [Path to icon]
```
If arguments are not provided, the script will search for them in the top level of your project directory.

In both cases, the banner and icon must be called 'banner' and 'icon' respectively. The scripts copy these files to your project directory, so running them without args after the first time will always find the banner and icon.

The scripts will create a ZIP file in the project directory with the same name as the containing folder.
