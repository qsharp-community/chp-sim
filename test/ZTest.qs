namespace ChpSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;
    
    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation ZRotateHXQubit () : Unit {
        use q = Qubit();
        H(q); //|+>
        Z(q); //|->
        AssertMeasurement([PauliX], [q], One, "Should be |->");
        Reset(q);
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation ZRotateHYQubit () : Unit {
        use q = Qubit();
        H(q); //|+>
        Z(q); //|->
        AssertMeasurementProbability([PauliY], [q], One, 0.5, "Should be |->", 1e-5 );
        Reset(q);
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation ZRotateHZQubit () : Unit {
        use q = Qubit();
        H(q); //|+>
        Z(q); //|->
        AssertMeasurementProbability([PauliZ], [q], One, 0.5, "Should be |->", 1e-5 );
        Reset(q);
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation ZRotateXQubit () : Unit {
        use q = Qubit();
        Z(q); //|0>
        AssertMeasurementProbability([PauliX], [q], One, 0.5, "Should be |0>", 1e-5 );
        Reset(q);
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation ZRotateYQubit () : Unit {
        use q = Qubit();
        Z(q); //|0>
        AssertMeasurementProbability([PauliY], [q], One, 0.5, "Should be |0>", 1e-5 );
        Reset(q);
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation ZRotateZQubit () : Unit {
        use q = Qubit();
        Z(q); //|0>
        AssertMeasurement([PauliZ], [q], Zero, "Should be |0>");
        Reset(q);
        Message("Test passed.");
    }
}
