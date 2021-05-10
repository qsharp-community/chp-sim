// <copyright file="TableauToTextEncoder.cs" company="https://qsharp.community/">
// Copyright (c) Sarah Kaiser. Licensed under the MIT License.
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
    /// Encodes the Tableau in text so it can be rendered by jupiter.
    /// </summary>
    public class TableauToTextEncoder : IResultEncoder
    {
        /// <summary>
        /// Gets the mimetype used for rendering that its text.
        /// </summary>
        public string MimeType => MimeTypes.PlainText;

        /// <summary>
        /// Returns the StabilizerTableau into the text as encoded data.
        /// </summary>
        /// <param name="displayable">Should be the StabilizerTableau.</param>
        /// <returns>Text encoded table.</returns>
        public EncodedData? Encode(object displayable)
        {
            if (displayable is StabilizerTableau tableau)
            {
                return tableau.Data.MatrixToPauliString(true).ToEncodedData();
            }
            else
            {
                return null;
            }
        }
    }
}
