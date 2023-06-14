<?php
  // defaults; backwards compatibility with Backpack 4.0 widgets
  $widget['wrapper']['class'] = $widget['wrapper']['class'] ?? $widget['wrapperClass'] ?? 'col-sm-6 col-lg-3';
?>

<?php echo $__env->renderWhen(!empty($widget['wrapper']), 'backpack::widgets.inc.wrapper_start', \Illuminate\Support\Arr::except(get_defined_vars(), ['__data', '__path'])); ?>
  <div class="<?php echo e($widget['class'] ?? 'card text-white bg-primary'); ?>">
    <div class="card-body">
      <?php if(isset($widget['value'])): ?>
      <div class="text-value"><?php echo $widget['value']; ?></div>
      <?php endif; ?>

      <?php if(isset($widget['description'])): ?>
      <div><?php echo $widget['description']; ?></div>
      <?php endif; ?>
      
      <?php if(isset($widget['progress'])): ?>
      <div class="progress progress-white progress-xs my-2">
        <div class="progress-bar" role="progressbar" style="width: <?php echo e($widget['progress']); ?>%" aria-valuenow="<?php echo e($widget['progress']); ?>" aria-valuemin="0" aria-valuemax="100"></div>
      </div>
      <?php endif; ?>
      
      <?php if(isset($widget['hint'])): ?>
      <small class="text-muted"><?php echo $widget['hint']; ?></small>
      <?php endif; ?>
    </div>
    
    <?php if(isset($widget['footer_link'])): ?>
    <div class="card-footer px-3 py-2">
      <a class="btn-block text-muted d-flex justify-content-between align-items-center" href="<?php echo e($widget['footer_link'] ?? '#'); ?>"><span class="small font-weight-bold"><?php echo e($widget['footer_text'] ?? 'View more'); ?></span><i class="la la-angle-right"></i></a>
    </div>
    <?php endif; ?>
  </div>
<?php echo $__env->renderWhen(!empty($widget['wrapper']), 'backpack::widgets.inc.wrapper_end', \Illuminate\Support\Arr::except(get_defined_vars(), ['__data', '__path'])); ?><?php /**PATH C:\xampp8.2.0\htdocs\HomeTherapist\HomeTherapistAdmin\vendor\backpack\crud\src\resources\views\base/widgets/progress.blade.php ENDPATH**/ ?>