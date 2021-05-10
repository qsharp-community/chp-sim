// <copyright file="StabilizerTableau.cs" company="https://qsharp.community/">
// Copyright (c) Sarah Kaiser. Licensed under the MIT License.
// </copyright>
// Adapted from State display encoders in the IQSharp project here:
// https://github.com/microsoft/iqsharp/blob/master/src/Jupyter/Visualization/StateDisplayEncoders.cs
#nullable enable

namespace QSharpCommunity.Simulators.Chp
{
    using System;

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
