using System;
using Android.Content;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;


namespace Pizza_Calculator
{

    [Register("Pizza_Calculator.ScrollAwareFABBehavior")]
    public class ScrollFabBehavior : CoordinatorLayout.Behavior
    {
        private readonly VisibilityListener _visibilityListener;

        public ScrollFabBehavior(Context context, IAttributeSet attrs)
        {
            _visibilityListener = new VisibilityListener();
        }

        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target,
            int nestedScrollAxes)
        {
            return nestedScrollAxes == ViewCompat.ScrollAxisVertical;
        }

        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed,
            int dxUnconsumed, int dyUnconsumed)
        {
            base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed);

            var fab = child.JavaCast<FloatingActionButton>();
            if (dyConsumed > 0 && fab.Visibility == ViewStates.Visible)
            {
                fab.Hide(_visibilityListener);
            }
            else if (dyConsumed < 0 && fab.Visibility != ViewStates.Visible)
            {
                for (var i = 0; i < coordinatorLayout.ChildCount; i++)
                {
                    if (coordinatorLayout.GetChildAt(i) is Snackbar.SnackbarLayout)
                    {
                        fab.Show();
                        return;
                    }
                }

                fab.TranslationY = 0.0f;
                fab.Show();
            }
        }

        private class VisibilityListener : FloatingActionButton.OnVisibilityChangedListener
        {
            public override void OnHidden(FloatingActionButton fab)
            {
                base.OnHidden(fab);
                fab.Visibility = ViewStates.Invisible;
            }
        }
    }

}