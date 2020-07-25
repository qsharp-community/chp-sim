// <copyright file="TableauToHtmlEncoder.cs" company="https://qsharp.community/">
// Copyright (c) Sarah Kaiser. All rights reserved.
// Licensed under the MIT License.
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
    /// Tableau visualizer for jupiter.
    /// </summary>
    public class TableauToHtmlEncoder : IResultEncoder
    {
        private readonly IConfigurationSource configurationSource;

        public TableauToHtmlEncoder(IConfigurationSource configurationSource)
        {
            this.configurationSource = configurationSource;
        }

        public string MimeType => MimeTypes.Html;

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
