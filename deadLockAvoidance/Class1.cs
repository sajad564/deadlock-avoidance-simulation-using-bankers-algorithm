using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deadLockAvoidance
{
    namespace BankersAlgorithm.Algorithms.Bankers
    {
        public enum RequestFailReason
        {
            AvailaibleResourceIsNotEnough = 1,
            ThereIsNoSafeSequence,
            RequestIsMoreThanRemainingNeed
        }
        public class ResourceRequestResponse
        {
            public bool Result { get; set; }
            public int[] SafeSequence { get; set; }
            public RequestFailReason? FailReason { get; set; }
            public int[,] Allocation { get; set; }
            public int[,] RemainingNeed { get; set; }
            public static ResourceRequestResponse BadState(RequestFailReason reason) => new ResourceRequestResponse { FailReason = reason };
            public static ResourceRequestResponse Success(int[,] allocation, int[,] remainingNeed, int[] safeSequence) => new ResourceRequestResponse { Allocation = allocation, RemainingNeed = remainingNeed, Result = true, SafeSequence = safeSequence };
        }
        public interface ISafetyCheck
        {
            (int[] safeSequence, bool isSafe) check(); 
        }
        public interface IResourceRequest
        {
            ResourceRequestResponse ProccessRequest(int[] resourceRequestVector, int proccessIndex);
        }
        public interface IDeadlockAvoidanceAlgorithm : ISafetyCheck, IResourceRequest { }
        internal class BankersImpImplementation : IDeadlockAvoidanceAlgorithm
        {
            private bool[] _Finish;
            private int[,] _Max;
            private int[,] _Allocation;
            private int[,] _Need => Extensions.Subtract(_Max, _Allocation);
            private int[] _Available;
            private int _ResourceCount = 0;
            private int _ProccessCount = 0;
            public int[,] getNeed() => (int[,])  _Need.Clone();
            public BankersImpImplementation(bool[] finish, int[,] max, int[,] allocation, int[] available, int resourceCount, int proccessCount)
            {
                _Finish = finish;
                _Max = max;
                _Allocation = allocation;
                _Available = available;
                _ResourceCount = resourceCount;
                _ProccessCount = proccessCount;
            }

            public (int[] safeSequence, bool isSafe) check()
            {
                var result = check(_Need, _Allocation, _Available, _Finish, _ProccessCount);
                reset();
                return result;
            }
            public void reset()
            {
                for (int i = 0; i < _Finish.Length; i++)
                    _Finish[i] = false;
            }

            private static (int[] safeSequence, bool isSafe) check(int[,] need, int[,] allocation, int[] available, bool[] finish, int proccessCount)
            {
                var work = (int[]) available.Clone();
                var find = true;
                var safeSequence = new int[proccessCount];
                var safeSequenceCurrentIndex = 0; 
                while (find)
                {
                    find = false;

                    for (int i = 0; i < need.GetLength(0); i++)
                    {
                        if (finish[i])
                            continue;
                        var needVector = need.GetRow(i);
                        if (needVector.IsLessThanEqual(work))
                        {
                            safeSequence[safeSequenceCurrentIndex] = i;
                            safeSequenceCurrentIndex++;
                            find = true;
                            finish[i] = true;
                            work = Extensions.Sum(work, allocation.GetRow(i));
                        }
                    }
                }
                bool _unSafe = finish.Where(c => c == false).Any();
                return (_unSafe ? null : safeSequence, !_unSafe);
            }
            public ResourceRequestResponse ProccessRequest(int[] resourceRequestVector, int proccessIndex)
            {
                if (!resourceRequestVector.IsLessThanEqual(_Available))
                    return ResourceRequestResponse.BadState(RequestFailReason.AvailaibleResourceIsNotEnough);
                if (!resourceRequestVector.IsLessThanEqual(_Need.GetRow(proccessIndex)))
                    return ResourceRequestResponse.BadState(RequestFailReason.RequestIsMoreThanRemainingNeed);
                //pretend allocation
                for (int i = 0; i < resourceRequestVector.Length; i++)
                {
                    _Need[proccessIndex, i] -= resourceRequestVector[i];
                    _Allocation[proccessIndex, i] += resourceRequestVector[i];
                    _Available[i] -= resourceRequestVector[i];
                }
                var checkResult = check(_Need, _Allocation, _Available, _Finish, _ProccessCount);
                reset();
                //unsafe : rollback to latest state
                if (!checkResult.isSafe)
                {
                    for (int i = 0; i < resourceRequestVector.Length; i++)
                    {
                        if (!checkResult.isSafe)
                        {
                            _Need[proccessIndex, i] += resourceRequestVector[i];
                            _Allocation[proccessIndex, i] -= resourceRequestVector[i];
                            _Available[i] += resourceRequestVector[i];
                        }
                    }
                    return ResourceRequestResponse.BadState(RequestFailReason.ThereIsNoSafeSequence);
                }
                return ResourceRequestResponse.Success(_Allocation, _Need, checkResult.safeSequence);

            }
        }
    }
}
