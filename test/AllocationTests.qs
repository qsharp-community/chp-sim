namespace CHPSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;
    
    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation AllocateOneQubit () : Unit {
        using (q = Qubit()) {
            AssertMeasurement([PauliZ], [q], Zero, "Newly allocated qubit must be in |0> state.");
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation BorrowOneQubit () : Unit {
        borrowing (q = Qubit()) {
            AssertMeasurement([PauliZ], [q], Zero, "Newly allocated qubit must be in |0> state.");
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation AllocateMultipleQubits () : Unit {
        using (register = Qubit[4]) {
            AssertAllZero(register);
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation BorrowingMultipleQubits () : Unit {
        borrowing (register = Qubit[4]) {
            AssertAllZero(register);
        }
        Message("Test passed.");
    }

    // Check that we don't go directly OOM with largish values
    @Test("chp.StabilizerSimulator")
    @Test("ToffoliSimulator")
    operation AllocateManyQubits () : Unit {
        using (register = Qubit[1024]) {
            AssertAllZero(register);
            //Make sure the compiler doesn't cheat at us'
            for(q in register)
            {
				X(q);
            }
            ResetAll(register);
        }
        Message("Test passed.");
    }

    // Check that we don't go directly OOM with largish values
    @Test("chp.StabilizerSimulator")
    @Test("ToffoliSimulator")
    operation BorrowingManyQubits () : Unit {
        borrowing (register = Qubit[1024]) {
            AssertAllZero(register);
            //Make sure the compiler doesn't cheat at us'
            for(q in register)
            {
				X(q);
            }
            ResetAll(register);
        }
        Message("Test passed.");
    }
}
