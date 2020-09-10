namespace ChpSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;
    
    ///
    /// Some Helper functions to make the unittest more readable
    ///

    // Brings from state |0> to |+>
    operation StatePlus (q: Qubit) : Unit {
        H(q);
    }

    // Brings from state |0> to |i>
    operation StateI (q: Qubit) : Unit {
        StatePlus(q); //|+>
        S(q); //|i>
    }
    
    // Brings from state |0> to |->
    operation StateMinus (q: Qubit) : Unit {
        StateI(q); //|i>
        S(q); //|->
    }

    // Brings from state |0> to |->
    operation StateMinusI (q: Qubit) : Unit {
        StateMinus(q); //|->
        S(q); //|-i>
    }

    // Brings from state |0> to |1>
    operation StateOne (q: Qubit) : Unit {
        StateMinus(q); //|->
        H(q); //|1>
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StatePlusXTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StatePlus(b);
            AssertMeasurementProbability([PauliX], [a], One, 0.5, "Should not be changed and still be |0>", 1e-5 );
            AssertMeasurement([PauliX], [b], Zero, "Should be |+>");
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StatePlusYTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StatePlus(b);
            AssertMeasurementProbability([PauliY], [a], One, 0.5, "Should not be changed and still be |0>", 1e-5 );
            AssertMeasurementProbability([PauliY], [b], One, 0.5, "Should be |+>", 1e-5 );
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StatePlusZTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StatePlus(b);
            AssertMeasurement([PauliZ], [a], Zero, "Should not be changed. Still be |0>");
            AssertMeasurementProbability([PauliZ], [b], One, 0.5, "Should be |+>", 1e-5 );
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateMinusXTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateMinus(b);
            AssertMeasurementProbability([PauliX], [a], One, 0.5, "Should not be changed and still be |0>", 1e-5 );
            AssertMeasurement([PauliX], [b], One, "Should be |->");
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateMinusYTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateMinus(b);
            AssertMeasurementProbability([PauliY], [a], One, 0.5, "Should not be changed and still be |0>", 1e-5 );
            AssertMeasurementProbability([PauliY], [b], One, 0.5, "Should be |->", 1e-5 );
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateMinusZTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateMinus(b);
            AssertMeasurement([PauliZ], [a], Zero, "Should not be changed. Still be |0>");
            AssertMeasurementProbability([PauliZ], [b], One, 0.5, "Should be |->", 1e-5 );
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateIXTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateI(b);
            AssertMeasurementProbability([PauliX], [a], One, 0.5, "Should not be changed and still be |0>", 1e-5 );
            AssertMeasurementProbability([PauliX], [b], One, 0.5, "Should be |i>", 1e-5 );
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateIYTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateI(b);
            AssertMeasurementProbability([PauliY], [a], One, 0.5, "Should not be changed and still be |0>", 1e-5 );
            AssertMeasurement([PauliY], [b], Zero, "Should be |i>");
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateIZTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateI(b);
            AssertMeasurement([PauliZ], [a], Zero, "Should not be changed. Still be |0>");
            AssertMeasurementProbability([PauliZ], [b], One, 0.5, "Should be |i>", 1e-5 );
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateMinusIXTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateMinusI(b);
            AssertMeasurementProbability([PauliX], [a], One, 0.5, "Should not be changed and still be |0>", 1e-5 );
            AssertMeasurementProbability([PauliX], [b], One, 0.5, "Should be |-i>", 1e-5 );
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateMinusIYTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateMinusI(b);
            AssertMeasurementProbability([PauliY], [a], One, 0.5, "Should not be changed and still be |0>", 1e-5 );
            AssertMeasurement([PauliY], [b], One, "Should be |-i>");
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateMinusIZTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateMinusI(b);
            AssertMeasurement([PauliZ], [a], Zero, "Should not be changed. Still be |0>");
            AssertMeasurementProbability([PauliZ], [b], One, 0.5, "Should be |-i>", 1e-5 );
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateOneXTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateOne(b);
            AssertMeasurementProbability([PauliX], [a], One, 0.5, "Should not be changed and still be |0>", 1e-5 );
            AssertMeasurementProbability([PauliX], [b], One, 0.5, "Should be |1>", 1e-5 );
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateOneYTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateOne(b);
            AssertMeasurementProbability([PauliY], [a], One, 0.5, "Should not be changed and still be |0>", 1e-5 );
            AssertMeasurementProbability([PauliY], [b], One, 0.5, "Should be |1>", 1e-5 );
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation StateOneZTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateOne(b);
            AssertMeasurement([PauliZ], [a], Zero, "Should not be changed. Still be |0>");
            AssertMeasurement([PauliZ], [b], One, "Should be |1>");
            Reset(b);
        }
        Message("Test passed.");
    }
}
