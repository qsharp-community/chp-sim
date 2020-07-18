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

TODO: Include logo/demo screenshot etc.

## Tech/framework used

**Built with:**

- [Quantum Development Kit](https://docs.microsoft.com/quantum/)
- [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- [Python](https://www.python.org/downloads/)
- [Visual Studio Code](https://code.visualstudio.com/) and [Visual Studio](https://visualstudio.microsoft.com/)
- [Jupyter Notebook](https://jupyter.org/)

## Features

What makes your project stand out?

## Code Example

TODO: Add more! Fix syntax highlighting


## Installation

TODO:
- [ ] Docker
- [ ] Binder
- [ ] Codespaces
- [ ] Remote development environment with VS Code
- [ ] local install

### Installation for the scratch pad notebooks

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
<!--Depending on the size of the project, if it is small and simple enough the reference docs can be added to the README. For medium size to larger projects it is important to at least provide a link to where the API reference docs live.-->

## Tests
TODO: Describe and show how to run the tests with code examples.

## How to use?
TODO: Link to user manual in docs
<!--If people like your project they’ll want to learn how they can use it. To do so include step by step guide to use your project.-->

## Contribute

Please see our [contributing guidelines](CONTRIBUTING.md) and our [code of conduct](CODE_OF_CONDUCT.md) before working on a contribution, thanks!

## Credits
- Primary developers: @crazy4pi314

#### Anything else that seems useful

If you need to check your intuition/understanding of the stabilizer formalism, you should check out these Python packages:
- [Quaec](http://www.cgranade.com/python-quaec/)
- [QuTiP](http://qutip.org/)

## License

MIT © [Sarah Kaiser](https://github.com/qsharp-community/qram/blob/master/LICENSE)
