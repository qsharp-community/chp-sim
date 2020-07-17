namespace CHPSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation HadamardQubits () : Unit {
        using (q = Qubit()) {
            H(q); //|+>
            S(q); //|i>
            S(q); //|->
            H(q); //|1>
            AssertMeasurement([PauliZ],[q], One, "Should make the rotations");
            Reset(q);
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation HadamardMultipleQubits () : Unit {
        using (register = Qubit[4]) {
            H(register[2]); //|+>
            S(register[2]); //|i>
            S(register[2]); //|->
            H(register[2]); //|1>
            AssertMeasurement([PauliZ],[register[0]], Zero, "first qubit shouldn't have been flipped");
            AssertMeasurement([PauliZ],[register[1]], Zero, "second qubit shouldn't have been flipped");
            AssertMeasurement([PauliZ],[register[2]], One, "third qubit should have been flipped");
            AssertMeasurement([PauliZ],[register[3]], Zero, "fourth qubit shouldn't have been flipped");
            Reset(register[2]);
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation HadamardProbMeasuredQubits () : Unit {
        using (q = Qubit()) {
            H(q); //|+>
            AssertMeasurementProbability([PauliZ],[q], One, 0.5, "Should make the rotations",0.4);
            Reset(q);
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation HadamardProbMeasuredMultipleQubits () : Unit {
        using (register = Qubit[4]) {
            H(register[2]); //|+>

            AssertMeasurement([PauliZ],[register[0]], Zero, "first qubit shouldn't have been flipped");
            AssertMeasurement([PauliZ],[register[1]], Zero, "second qubit shouldn't have been flipped");
            AssertMeasurementProbability([PauliZ],[register[2]], One, 0.5, "should be in superposition", 0.4);
            AssertMeasurement([PauliZ],[register[3]], Zero, "fourth qubit shouldn't have been flipped");
            Reset(register[2]);
        }
        Message("Test passed.");
    }
}
