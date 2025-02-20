
namespace DotNetToolbox.Sequencers;

public sealed class NumericSequencer : NumericSequencer<NumericSequencer>;

public class NumericSequencer<TSequence>(uint start)
    : Sequencer<TSequence, uint>(start),
      INumericSequencer
    where TSequence : NumericSequencer<TSequence> {
    public NumericSequencer()
        : this(0) {
    }

    protected override bool TryGenerateNext(uint current, out uint next) {
        next = current;
        try {
            ++next;
            return true;
        }
        catch {
            return false;
        }
    }
}
