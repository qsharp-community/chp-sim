// Copyright (c) Sarah Kaiser. All rights reserved.
// Licensed under the MIT License.
// Adapted from State display encoders in the IQSharp project here: 
// https://github.com/microsoft/iqsharp/blob/master/src/Jupyter/Visualization/StateDisplayEncoders.cs
#nullable enable

using System;
using Microsoft.Jupyter.Core;
using Microsoft.Quantum.IQSharp.Jupyter;

namespace QSharpCommunity.Simulators.Chp
{
    public class StabilizerTableau
    {
        public bool[,] Data { get; set; }
    }

    public class TableauToTextEncoder : IResultEncoder
    {
        public string MimeType => MimeTypes.PlainText;

        public EncodedData? Encode(object displayable)
        {
            if (displayable is StabilizerTableau tableau)
            {
                return tableau.Data.MatrixToString(true).ToEncodedData();
            }
            else return null;
        }
    }

    public class TableauToHtmlEncoder : IResultEncoder
    {
        public string MimeType => MimeTypes.Html;
        private IConfigurationSource configurationSource;
        public TableauToHtmlEncoder(IConfigurationSource configurationSource)
        {
            this.configurationSource = configurationSource;
        }
        public EncodedData? Encode(object displayable)
        {
            if (displayable is StabilizerTableau tableau)
            {
                var showDestabilizers =
                    configurationSource.Configuration.TryGetValue("chp.showDestabilizers", out var token)
                    ? token.ToObject<bool>()
                    : false;

                var nQubits = tableau.Data.GetLength(0) / 2;
                var tableFormat = new String('c', nQubits);
                var tableRows = tableau.Data.MatrixToLatexString(showDestabilizers);

                var outputTable = $@"
                $\left(\begin{{array}}{{{tableFormat}|{tableFormat}|c}}
                {tableRows}
                \end{{array}} \right)$";

                return outputTable.ToEncodedData();
            }
            else return null;
        }
    }
}
