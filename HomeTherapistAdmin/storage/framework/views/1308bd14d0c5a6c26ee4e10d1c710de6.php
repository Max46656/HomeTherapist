<!-- Users, Roles, Permissions -->
<li class="nav-item nav-dropdown">
    <a class="nav-link nav-dropdown-toggle" href="#"><i class="nav-icon la la-users"></i> Authentication</a>
    <ul class="nav-dropdown-items">
        <li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('user')); ?>"><i class="nav-icon la la-user"></i>
                <span>Users</span></a></li>
        <li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('role')); ?>"><i
                    class="nav-icon la la-id-badge"></i>
                <span>Roles</span></a></li>
        <li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('permission')); ?>"><i
                    class="nav-icon la la-key"></i>
                <span>Permissions</span></a></li>
    </ul>
</li>


<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('dashboard')); ?>"><i class="la la-home nav-icon"></i> <?php echo e(trans('backpack::base.dashboard')); ?></a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('appointment')); ?>"><i
            class="nav-icon la la-question"></i> Appointments</a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('appointment-detail')); ?>"><i
            class="nav-icon la la-question"></i> Appointment details</a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('article')); ?>"><i class="nav-icon la la-question"></i>
        Articles</a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('calendar')); ?>"><i class="nav-icon la la-question"></i>
        Calendars</a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('feedback')); ?>"><i class="nav-icon la la-question"></i>
        Feedback</a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('order')); ?>"><i class="nav-icon la la-question"></i>
        Orders</a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('order-detail')); ?>"><i
            class="nav-icon la la-question"></i> Order details</a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('service')); ?>"><i class="nav-icon la la-question"></i>
        Services</a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('therapist-open-services')); ?>"><i
            class="nav-icon la la-question"></i> Therapist open services</a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('therapist-open-time')); ?>"><i
            class="nav-icon la la-question"></i> Therapist open times</a></li>
<li class="nav-item"><a class="nav-link" href="<?php echo e(backpack_url('user')); ?>"><i class="nav-icon la la-question"></i>
        Users</a></li><?php /**PATH C:\xampp8.2.0\htdocs\HomeTherapist\HomeTherapistAdmin\resources\views/vendor/backpack/base/inc/sidebar_content.blade.php ENDPATH**/ ?>