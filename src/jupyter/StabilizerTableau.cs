// <copyright file="StabilizerTableau.cs" company="https://qsharp.community/">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// Adapted from State display encoders in the IQSharp project here:
// https://github.com/microsoft/iqsharp/blob/master/src/Jupyter/Visualization/StateDisplayEncoders.cs
#nullable enable

namespace QSharpCommunity.Simulators.Chp
{
    using System;
    using Microsoft.Jupyter.Core;
    using Microsoft.Quantum.IQSharp.Jupyter;

    /// <summary>
    /// Representation of the Tableau.
    /// </summary>
    public class StabilizerTableau
    {
        /// <summary>
        /// Gets or sets the data to be shown in the Tableau format.
        /// </summary>
        public bool[,] Data { get; set; } = new bool[1, 1];
    }
}
