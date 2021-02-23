namespace ChpSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;
    
    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation AllocateOneQubit () : Unit {
        use q = Qubit();
        AssertMeasurement([PauliZ], [q], Zero, "Newly allocated qubit must be in |0> state.");
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation BorrowOneQubit () : Unit {
        borrow q = Qubit();
        AssertMeasurement([PauliZ], [q], Zero, "Newly allocated qubit must be in |0> state.");
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation AllocateMultipleQubits () : Unit {
        use register = Qubit[4];
            AssertAllZero(register);
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation BorrowingMultipleQubits () : Unit {
        borrow register = Qubit[4];
            AssertAllZero(register);
        Message("Test passed.");
    }

    // Check that we don't go directly OOM with largish values
    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("ToffoliSimulator")
    operation AllocateManyQubits () : Unit {
        use register = Qubit[1024];
        AssertAllZero(register);
        //Make sure the compiler doesn't cheat at us'
        for q in register
        {
            X(q);
        }
        ResetAll(register);
        Message("Test passed.");
    }

    // Check that we don't go directly OOM with largish values
    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("ToffoliSimulator")
    operation BorrowingManyQubits () : Unit {
        borrow register = Qubit[1024];
        AssertAllZero(register);
        //Make sure the compiler doesn't cheat at us'
        for q in register
        {
            X(q);
        }
        ResetAll(register);
        Message("Test passed.");
    }
}
