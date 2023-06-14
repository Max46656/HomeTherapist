<?php echo $__env->renderWhen(!empty($widget['wrapper']), 'backpack::widgets.inc.wrapper_start', \Illuminate\Support\Arr::except(get_defined_vars(), ['__data', '__path'])); ?>

<div 
	<?php if(count($widget) > 2): ?>
	    <?php $__currentLoopData = $widget; $__env->addLoop($__currentLoopData); foreach($__currentLoopData as $attribute => $value): $__env->incrementLoopIndices(); $loop = $__env->getLastLoop(); ?>
	        <?php if(is_string($attribute) && $attribute!='content' && $attribute!='type'): ?>
	            <?php echo e($attribute); ?>="<?php echo e($value); ?>"
	        <?php endif; ?>
	    <?php endforeach; $__env->popLoop(); $loop = $__env->getLastLoop(); ?>
	<?php endif; ?>
	>

	<?php if(isset($widget['content'])): ?>
		<?php echo $__env->make('backpack::inc.widgets', [ 'widgets' => $widget['content'] ], \Illuminate\Support\Arr::except(get_defined_vars(), ['__data', '__path']))->render(); ?>
	<?php endif; ?>

</div>

<?php echo $__env->renderWhen(!empty($widget['wrapper']), 'backpack::widgets.inc.wrapper_end', \Illuminate\Support\Arr::except(get_defined_vars(), ['__data', '__path'])); ?><?php /**PATH C:\xampp8.2.0\htdocs\HomeTherapist\HomeTherapistAdmin\vendor\backpack\crud\src\resources\views\base/widgets/div.blade.php ENDPATH**/ ?>