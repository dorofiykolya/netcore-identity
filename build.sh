#!/usr/bin/env bash

SolutionPath=src/Identity.sln

dotnet clean "$SolutionPath"
dotnet build "$SolutionPath"