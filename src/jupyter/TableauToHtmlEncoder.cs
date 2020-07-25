// <copyright file="TableauToHtmlEncoder.cs" company="https://qsharp.community/">
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
    /// Encodes the Tableau in HTML so it can be rendered by jupiter.
    /// </summary>
    public class TableauToHtmlEncoder : IResultEncoder
    {
        private readonly IConfigurationSource configurationSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableauToHtmlEncoder"/> class.
        /// </summary>
        /// <param name="configurationSource">Settings to be used.</param>
        public TableauToHtmlEncoder(IConfigurationSource configurationSource)
        {
            this.configurationSource = configurationSource;
        }

        /// <summary>
        /// Gets the mimetype used for rendering that its html.
        /// </summary>
        public string MimeType => MimeTypes.Html;

        /// <summary>
        /// Returns the StabilizerTableau into the text as encoded data.
        /// </summary>
        /// <param name="displayable">Should be the StabilizerTableau.</param>
        /// <returns>Text encoded table.</returns>
        public EncodedData? Encode(object displayable)
        {
            if (displayable is StabilizerTableau tableau)
            {
                var showDestabilizers =
                    this.configurationSource.Configuration.TryGetValue("chp.showDestabilizers", out var token) && token.ToObject<bool>();

                var nQubits = tableau.Data.GetLength(0) / 2;
                var tableFormat = new string('c', nQubits);
                var tableRows = tableau.Data.MatrixToLatexString(showDestabilizers);

                var outputTable = $@"
                $\left(\begin{{array}}{{{tableFormat}|{tableFormat}|c}}
                {tableRows}
                \end{{array}} \right)$";

                return outputTable.ToEncodedData();
            }
            else
            {
                return null;
            }
        }
    }
}
