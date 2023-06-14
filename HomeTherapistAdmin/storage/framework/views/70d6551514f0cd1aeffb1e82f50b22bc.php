<?php
	$widget['wrapper']['element'] = $widget['wrapper']['element'] ?? 'div';
	$widget['wrapper']['class'] = $widget['wrapper']['class'] ?? "col-sm-6 col-md-4";

    // each wrapper attribute can be a callback or a string
    // for those that are callbacks, run the callbacks to get the final string to use
    foreach($widget['wrapper'] as $attribute => $value) {
        $widget['wrapper'][$attribute] = (!is_string($value) && is_callable($value) ? $value() : $value) ?? '';
    }
?>

<<?php echo e($widget['wrapper']['element'] ?? 'div'); ?>

<?php $__currentLoopData = Arr::where($widget['wrapper'],function($value, $key) { return $key != 'element'; }); $__env->addLoop($__currentLoopData); foreach($__currentLoopData as $element => $value): $__env->incrementLoopIndices(); $loop = $__env->getLastLoop(); ?>
    <?php echo e($element); ?>="<?php echo e($value); ?>"
<?php endforeach; $__env->popLoop(); $loop = $__env->getLastLoop(); ?>
><?php /**PATH C:\xampp8.2.0\htdocs\HomeTherapist\HomeTherapistAdmin\vendor\backpack\crud\src\resources\views\base/widgets/inc/wrapper_start.blade.php ENDPATH**/ ?>