# Dotnet Raycast Demo

## Description

A fake 3D environment created using raycasting

Here you can see a small minimap of the level as well as the rendered 3D:

![screenshot](./img/screenshot.png)

## Dependencies

 - SFML Libraries
 
   + For Linux: libcsfml-dev for Debian Systems or csfml for Arch
   
   + For Windows: Not needed
   
 - dotnet core 3.1 sdk
 
 - GNU Make (MinGW on Windows)

## Build

- Linux:

   + Run `make LINUX=1`
 
 - Windows
 
   + Run `mingw32-make <os>=1` where <os> can be WIN32 or WIN64 depending on your OS
