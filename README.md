# CHP simulator for Q\#

This library implements a new classical simulator for Q# that utilizes the CHP stabilizer classical sub-theory.
Based on the Python implementation by @Strilanc here: https://github.com/Strilanc/python-chp-stabilizer-simulator.

## Motivation

Simulating quantum systems on a classical computer is hard to do in full generalization, as the resources needed scale exponentially with the number of qubits you want to simulate (up to about ~30 qubits on a typical machine).
If you impose some constraints on the operations you can do in your programs, you can use a different kind of simulator that allows you to simulate hundreds of qubits by using what is called a _classical sub-theory_ of quantum physics.
There are a variety of sub-theories that each have a different set of constraints you have to work with to leverage it.
This repo implements a simulator for the CHP (CNOT, Hadamard, and Phase) classical sub-theory which helpfully has the constraint baked into the name.
In this simulator, all operations it supports have to be comprised of (or decomposable to) only the operations CNOT, Hadamard, and Phase.
If you run a program with this simulator as the target, and you ask for an operation that is not supported, the simulator will throw and exception.

## Build status

![Build](https://github.com/qsharp-community/chp-sim/workflows/Build/badge.svg)

## Code style

[![q# code style](https://img.shields.io/badge/code%20style-Q%23-blue)](https://docs.microsoft.com/en-us/quantum/contributing/style-guide?tabs=guidance)
[![q# APIcode style](https://img.shields.io/badge/code%20style-Q%23%20API-ff69b4)](https://docs.microsoft.com/en-us/quantum/contributing/style-guide?tabs=guidance)
[![c# APIcode style](https://img.shields.io/badge/code%20style-C%23-lightgrey)](https://docs.microsoft.com/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
[![CoC](https://img.shields.io/badge/code%20of%20conduct-contributor%20covenant-yellow)](CODE_OF_CONDUCT.md)

## Screenshots

![Jupyter screenshot](/docs/media/jupyter-magic.png)

## Tech/framework used

**Built with:**

- [Quantum Development Kit](https://docs.microsoft.com/quantum/)
- [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- [Python](https://www.python.org/downloads/)
- [Visual Studio Code](https://code.visualstudio.com/) and [Visual Studio](https://visualstudio.microsoft.com/)
- [Jupyter Notebook](https://jupyter.org/)

## Features

Anywhere you can use Q#, you can use this simulator!

- Python host program
- Jupyter Notebooks
- Stand-alone command line application
- C#/F# host program

## Code Example

TODO: Add more! Fix syntax highlighting


## Installation

To use this simulator, there is no reqired installation (unless you want to develop the simulator locally).
See the section below on how to use the simulator to see how to add it to your projects.

More generally, for complete and up-to-date ways to install the Quantum Development Kit (including Q# tooling) see [the official Q# docs](https://docs.microsoft.com/quantum/quickstarts/).


### Python installation the scratch pad notebooks (Python packages only, no Q#/Chp simulator)

1. Start with your favorite Anaconda or Miniconda install
2. Use the included `environment-qutip.yml` or  `environment-quaec.yml` to create a conda envionment.
    ```bash
    $ conda env create -f <NAME OF FILE.yml>
    ```
3. Activate the environment and then start the Jupyter Notebook. The `<ENV NAME>` will be at the top of the yml file you used to create the environment.
    ```bash
    $ conda activate <ENV NAME>
    $ jupyter notebook
    ```
    Then you should have Jupyter Notebook launch and you can use the corresponding notebook at the environment root for scratch work/testing your understanding of stabilizers.


## API Reference

TODO: Complete once compiler extension is finalized for scraping API docs

## Tests

The tests for this library all live in the `tests` directory.
To run the tests, navigate to that directory and run `dotnet test` and the .NET project will find and run all the tests in this directory.
If you are adding new features or functionality, make sure to add some tests to either the existing files, or make a new one.
For more info on writing tests for Q#, check out the [official Q# docs](https://docs.microsoft.com/quantum/user-guide/using-qsharp/testing-debugging).

## How to use?

### You want to use a published version of the simulator:

Just add a `PackageReference` to you project file with the following line:

```xml
<PackageReference Include="QSharpCommunity.Simulators.Chp" Version="X.X.X" />
```
or this console command:

```bash
$ dotnet add package QSharpCommunity.Simulators.Chp -v X.X.X
```

In both cases you can omit the version information to use the latest available Chp simulator on https://NuGet.org.



### You want to develop the simulator (locally build and use simulator):

The basic idea in this case is build locally a version of the nuget package for the simulator and then put it in a folder that is a known source to nuget.

0. Remove any previous copies of the package from your local nuget feed (you likely picked this location), and global nuget cache (default path on Windows 10 for the cache is below):

```Powershell
> rm C:\Users\skais\nuget-packages\QSharpCommunity.Simulators.Chp.X.X.X.nupkg
> rm C:\Users\skais\.nuget\packages\QSharpCommunity.Simulators.Chp\
```

1. Build the package for the Chp Simulator:

```Powershell
> cd src
> dotnet pack
```

2. Copy the package to your local nuget source (a location you selected, an example one is below). The `X` in the name are the place holder for the version number you are building (should be generated by the previous step).

```Powershell
> cp .\bin\Debug\QSharpCommunity.Simulators.Chp.X.X.X.nupkg 'C:\Users\skais\nuget-packages\'
```


## Contribute

Please see our [contributing guidelines](CONTRIBUTING.md) and our [code of conduct](CODE_OF_CONDUCT.md) before working on a contribution, thanks!

## Credits

- Primary developers: @crazy4pi314 @RolfHuisman

#### Anything else that seems useful

If you need to check your intuition/understanding of the stabilizer formalism, you should check out these Python packages:
- [Quaec](http://www.cgranade.com/python-quaec/)
- [QuTiP](http://qutip.org/)

## License

MIT © [Sarah Kaiser](https://github.com/qsharp-community/chp-sim/blob/master/LICENSE)
