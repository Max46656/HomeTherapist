<?php
	// preserve backwards compatibility with Widgets in Backpack 4.0
	$widget['wrapper']['class'] = $widget['wrapper']['class'] ?? $widget['wrapperClass'] ?? 'col-sm-6 col-md-4';
?>

<?php echo $__env->renderWhen(!empty($widget['wrapper']), 'backpack::widgets.inc.wrapper_start', \Illuminate\Support\Arr::except(get_defined_vars(), ['__data', '__path'])); ?>
	<div class="<?php echo e($widget['class'] ?? 'card'); ?>">
		<?php if(isset($widget['content'])): ?>
			<?php if(isset($widget['content']['header'])): ?>
				<div class="card-header"><?php echo $widget['content']['header']; ?></div>
			<?php endif; ?>
			<div class="card-body"><?php echo $widget['content']['body']; ?></div>
	  	<?php endif; ?>
	</div>
<?php echo $__env->renderWhen(!empty($widget['wrapper']), 'backpack::widgets.inc.wrapper_end', \Illuminate\Support\Arr::except(get_defined_vars(), ['__data', '__path'])); ?><?php /**PATH C:\xampp8.2.0\htdocs\HomeTherapist\HomeTherapistAdmin\vendor\backpack\crud\src\resources\views\base/widgets/card.blade.php ENDPATH**/ ?>