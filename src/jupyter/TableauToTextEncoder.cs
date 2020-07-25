// <copyright file="TableauToTextEncoder.cs" company="https://qsharp.community/">
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

    public class TableauToTextEncoder : IResultEncoder
    {
        public string MimeType => MimeTypes.PlainText;

        public EncodedData? Encode(object displayable)
        {
            if (displayable is StabilizerTableau tableau)
            {
                return tableau.Data.MatrixToString(true).ToEncodedData();
            }
            else
            {
                return null;
            }
        }
    }
}
