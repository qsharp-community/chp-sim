// Copyright (c) Sarah Kaiser. All rights reserved.
// Licensed under the MIT License.

namespace QSharpCommunity.Simulators.Chp.Sample {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Diagnostics;
    
    @EntryPoint()
    operation Test() : Unit {
        using (q = Qubit()) {
            DumpMachine();
            let resultZero = M(q);
            EqualityFactR(resultZero, Zero, "X didn't return correct measurement.");            
            X(q);
            DumpMachine();
            let resultOne = M(q);
            EqualityFactR(resultOne, One, "X didn't return correct measurement.");
            DumpMachine();
            if (M(q) == One) { X(q); }
            DumpMachine();
        }
    }
}
