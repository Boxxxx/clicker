using UnityEngine;
using System.Collections;

namespace Utils {
    public static class TimeUtil {
        public static string Format(float time) {
            return string.Format("{0:D2}", (int)(time) / 60) + ":" + string.Format("{0:D2}", (int)(time) % 60);
        }
    }

    public class Timestamp {
        private float _time;

        public Timestamp() {
            Update();
        }
        public Timestamp(float time) {
            _time = time;
        }
        public Timestamp(Timestamp other) {
            _time = other._time;
        }

        public void Update() {
            _time = GetCurrentTime();
        }
        public float Elapsed() {
            return GetCurrentTime() - _time;
        }
        public float ElapsedInMillisecond() {
            return Elapsed() * 1000f;
        }
        public bool IsElapsed(float interval) {
            return Elapsed() >= interval;
        }

        private float GetCurrentTime() {
            return Time.time;
        }
    }

    public class Timer {
        private float _time = 0;
        private Timestamp _timestamp = new Timestamp();
        private bool _isPaused = true;

        public float Time {
            get { return Elapsed(); }
            set {
                Pause();
                _time = value;
                Refresh();
            }
        }

        public bool IsPaused { get { return _isPaused; } }

        public void Start() {
            Refresh();
            _time = 0;
            _isPaused = false;
        }

        public void Reset() {
            Pause();
            _time = 0;
        }

        public void Pause() {
            if (!_isPaused) {
                Update();
                _isPaused = true;
            }
        }

        public void Resume() {
            if (_isPaused) {
                Refresh();
                _isPaused = false;
            }
        }

        public float GetTimeInMillisecond() {
            return Elapsed() * 1000f;
        }

        public float Elapsed() {
            if (_isPaused) {
                return _time;
            }
            else {
                return _time + _timestamp.Elapsed();
            }
        }

        private void Refresh() {
            _timestamp = new Timestamp();
        }

        private void Update() {
            _time += _timestamp.Elapsed();
        }

    }
}
