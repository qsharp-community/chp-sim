namespace ChpSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation PauliXFlipOneQubit () : Unit {
        use q = Qubit();
        X(q);
        AssertMeasurement([PauliZ],[q], One, "Qubit should have been flipped");
        Reset(q);
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation PauliXFlipSecondQubit () : Unit {
        use register = Qubit[4];
        X(register[1]);
        AssertMeasurement([PauliZ],[register[0]], Zero, "first qubit shouldn't have been flipped");
        AssertMeasurement([PauliZ],[register[1]], One, "second qubit should have been flipped");
        AssertMeasurement([PauliZ],[register[2]], Zero, "third qubit shouldn't have been flipped");
        AssertMeasurement([PauliZ],[register[3]], Zero, "fourth qubit shouldn't have been flipped");
        ResetAll(register);
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation PauliXPairQubit () : Unit {
        use (left,right) = (Qubit(),Qubit());
        X(right);
        AssertMeasurement([PauliZ],[left], Zero, "first qubit shouldn't have been flipped");
        AssertMeasurement([PauliZ],[right], One, "second qubit should have been flipped");

        Reset(right);
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation PauliXFlipAfterHadamardQubit () : Unit {
        use q = Qubit();
        H(q); // |+>
        X(q); // |+>
        H(q); // |0>
        AssertMeasurement([PauliZ],[q], Zero, "Qubit shouldn't have been flipped");
        Reset(q);
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation FlippedPauliXFlipAfterHadamardQubit () : Unit {
        use q = Qubit();
        X(q); // |1>
        H(q); // |->
        X(q); // |->
        H(q); // |1>
        AssertMeasurement([PauliZ],[q], One, "Qubit shouldn't have been flipped");
        Reset(q);
        Message("Test passed.");
    }
}
